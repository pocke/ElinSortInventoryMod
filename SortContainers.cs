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

        foreach (var thing in backpack.Inv.Container.things)
        {
            if (thing.IsContainer)
            {
                Logger.LogInfo($"Container: {thing}");
                var ui = GetUIInventoryForThing(thing);
                if (ui != null)
                {
                    ui.Sort();
                }
            } else 
            {
                Logger.LogInfo($"Not a container: {thing}");
            }
        }

        SE.Click();
    }

    private LayerInventory GetPCBackpack()
    {
        return LayerInventory.listInv.First(layer => layer.mainInv);
    }

    public static UIInventory GetUIInventoryForThing(Thing t)
    {
        foreach (LayerInventory item in LayerInventory.listInv)
        {
            foreach (UIInventory inventory in item.invs)
            {
                if (inventory.owner.Container.Thing == t)
                {
                    return inventory;
                }
            }
        }
        return null;
    }
}
