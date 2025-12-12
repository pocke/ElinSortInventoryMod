using System.Collections.Generic;
using System.Linq;

namespace SortInventory;

class Sorter
{

    public static IEnumerable<Thing> GetContainersFromBackpack(LayerInventory backpack)
    {
        var containersInBackpack = backpack.Inv.Container.things.Where(t => IsOpenContainer(t));
        var containersInToolBelt = new List<Thing>();
        foreach (var belt in containersInBackpack.Where(t => t.things.owner.trait is TraitToolBelt))
        {
            containersInToolBelt.AddRange(belt.things.Where(t => IsOpenContainer(t)));
        }

        var containers = containersInBackpack.Where(t => t.things.owner.trait is not TraitToolBelt).Concat(containersInToolBelt);

        return containers.Where(container =>
        {
            var name = container.Name;
            foreach (var pattern in Settings.IgnoredContainerPattern)
            {
                if (name.Contains(pattern))
                {
                    SortInventory.Log($"Ignoring container '{name}' matching pattern '{pattern}'");
                    return false;
                }
            }
            return true;
        });
    }

    public static UIInventory GetUIInventoryForCard(Card card, LayerInventory backpack)
    {
        if (card == EClass.pc)
        {
            return backpack.invs[0];
        }

        foreach (LayerInventory item in LayerInventory.listInv)
        {
            foreach (UIInventory inventory in item.invs)
            {
                if (inventory.owner.Container.Thing == card)
                {
                    return inventory;
                }
            }
        }
        return null;
    }

    public static IEnumerable<UIInventory> GetUIInventoryForThings(IEnumerable<Thing> things)
    {
        foreach (LayerInventory item in LayerInventory.listInv)
        {
            foreach (UIInventory inventory in item.invs)
            {
                if (things.Contains(inventory.owner.Container.Thing))
                {
                    yield return inventory;
                }
            }
        }
    }

    private static bool IsContainerLocked(Thing container)
    {
        return container.trait.owner.c_lockLv > 0;
    }

    private static bool IsOpenContainer(Thing t)
    {
        return t.IsContainer && !IsContainerLocked(t);
    }
}
