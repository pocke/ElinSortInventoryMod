using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SortInventory;

class ConcatenatedSorter : Sorter
{
    class HashSetComparer : IEqualityComparer<Window.SaveData>
    {
        public bool Equals(Window.SaveData x, Window.SaveData y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }
            return x.cats.SetEquals(y.cats) && (x.filter ?? "") == (y.filter ?? "") && x.sharedType == y.sharedType;
        }

        public int GetHashCode(Window.SaveData obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int hash = 0;

            if (obj.filter != null && obj.filter != "")
            {
                hash ^= obj.filter.GetHashCode();
            }

            foreach (var item in obj.cats)
            {
                hash ^= item.GetHashCode();
            }

            return hash;
        }
    }

    public static void Sort(LayerInventory backpack)
    {
        var containers = GetContainersFromBackpack(backpack).ToList<Card>();
        containers.Add(EClass.pc);

        var grouped = containers.GroupBy(t => t.GetWindowSaveData(), new HashSetComparer());
        foreach (var group in grouped)
        {
            SortInventory.Log($"Sorting group with {group.Count()} containers");

            var ui = GetUIInventoryForCard(group.First(), backpack);
            if (group.Count() == 1)
            {
                ui.Sort();
                continue;
            }

            var tmpContainer = new List<Thing>();
            foreach (var container in group)
            {
                foreach (var t in container.things.Where(t => !t.isEquipped && !t.IsHotItem && !t.IsContainer).ToList())
                {
                    // TODO: TryStack
                    SortInventory.Log($"Moving {t} from {container} to tmpContainer");
                    tmpContainer.Add(t);

                    container.RemoveThing(t);
                }
            }

            tmpContainer.Sort(SortMode(ui), SortOrder(ui));

            var dests = SortContainers(group).ToList();
            var dest = dests[0];
            dests.RemoveAt(0);

            foreach (var t in tmpContainer)
            {
                while (dest.things.IsFull() && dests.Count > 0)
                {
                    dest = dests[0];
                    dests.RemoveAt(0);
                }
                dest.AddThing(t);
            }
        }
    }

    public static IEnumerable<Card> SortContainers(IEnumerable<Card> containers)
    {
        return containers.OrderBy(c => {
            var saveData = c.GetWindowSaveData();
            int priority = 0;
            if (saveData != null)
            {
                priority = -saveData.priority;
            }
            return priority;
        }).ThenBy(c => c == EClass.pc ? -1 :  c.invX);
    }

    public static UIList.SortMode SortMode(UIInventory ui)
    {
        return EMono.core.config.game.sortEach ? ui.window.saveData.sortMode : EMono.player.pref.sortInv;
    }

    public static bool SortOrder(UIInventory ui)
    {
        return EMono.core.config.game.sortEach ? ui.window.saveData.sort_ascending : EMono.player.pref.sort_ascending;
    }
}
