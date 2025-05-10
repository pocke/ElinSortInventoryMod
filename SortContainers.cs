using System.Collections.Generic;
using System.Configuration;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Linq; // LINQを使用するために追加

namespace SortContainers;

internal static class ModInfo
{
    internal const string Guid = "me.pocke.sort-containers";
    internal const string Name = "Sort Containers";
    internal const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class SortContainers : BaseUnityPlugin
{
    private void Awake()
    {
        Settings.keyCode = Config.Bind("Settings", "KeyCode", KeyCode.S, new ConfigDescription("Key to sort containers", null, null));
        Settings.keyCodeMod = Config.Bind("Settings", "KeyCodeMod", KeyCode.LeftAlt, new ConfigDescription("Modifier key to sort containers. If None is specified, it does not require a modifier key.", null, null));
    }

    private void Update()
    {
        if (!EClass.core.IsGameStarted)
        {
            return;
        }

        if ((Settings.KeyCodeMod == KeyCode.None || Input.GetKey(Settings.KeyCodeMod)) && Input.GetKeyDown(Settings.KeyCode))
        {
            SortAllContainers();
        }
    }

    private void SortAllContainers()
    {
        var backpack = GetPCBackpack();
        Logger.LogInfo($"Backpack: {backpack}");
        backpack.invs[0].Sort();

        var containers = backpack.Inv.Container.things.Where(t => t.IsContainer);
        var uis = GetUIInventoryForThings(containers);
        foreach (var ui in uis)
        {
            Logger.LogInfo($"UIInventory: {ui}");
            ui.Sort();
        }

        SE.Click();
    }

    private LayerInventory GetPCBackpack()
    {
        return LayerInventory.listInv.First(layer => layer.mainInv);
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
