using System.Collections.Generic;
using System.Linq;

namespace SortContainers;

class IndependentSorter : Sorter
{
    public static void Sort(LayerInventory backpack)
    {
        backpack.invs[0].Sort();

        var containers = GetContainersFromBackpack(backpack);
        var uis = GetUIInventoryForThings(containers);

        foreach (var ui in uis)
        {
            SortContainers.Log($"UIInventory: {ui}");
            ui.Sort();
        }
    }

}
