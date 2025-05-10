using System.Collections.Generic;
using System.Linq;

class Sorter
{

    public static IEnumerable<Thing> GetContainersFromBackpack(LayerInventory backpack) {
        return backpack.Inv.Container.things.Where(t => t.IsContainer && t.things.owner.trait is not TraitToolBelt).ToList();
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
}
