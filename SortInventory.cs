using System.Collections.Generic;
using System.Configuration;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace SortInventory;

internal static class ModInfo
{
    internal const string Guid = "me.pocke.sort-inventory";
    internal const string Name = "Sort Inventory";
    internal const string Version = "1.0.2";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
internal class SortInventory : BaseUnityPlugin
{
    internal static SortInventory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Settings.keyCode = Config.Bind("Settings", "KeyCode", KeyCode.S, new ConfigDescription("Key to sort the inventory", null, null));
        Settings.keyCodeMod = Config.Bind("Settings", "KeyCodeMod", KeyCode.LeftAlt, new ConfigDescription("Modifier key to sort the inventory. If None is specified, it does not require a modifier key.", null, null));
        Settings.concatContainers = Config.Bind("Settings", "ConcatContainers", false, new ConfigDescription("If true, it treats containers having the same settings as one container. If false (default), it sorts containers independently.", null, null));
        Settings.ignoredContainerPattern = Config.Bind("Settings", "IgnoredContainerPattern", "", new ConfigDescription("If specified, containers whose names match this pattern will be ignored when sorting. The patterns are a comma-separated strings.", null, null));
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
        if (backpack == null)
        {
            Log("No backpack found.");
            SE.CancelAction();
            return;
        }

        if (Settings.ConcatContainers)
        {
            ConcatenatedSorter.Sort(backpack);
        }
        else
        {
            IndependentSorter.Sort(backpack);
        }

        SE.Click();
    }

    private LayerInventory GetPCBackpack()
    {
        return LayerInventory.listInv.FirstOrDefault(layer => layer.mainInv);
    }

    internal static void Log(object payload)
    {
        Instance!.Logger.LogInfo(payload);
    }
}
