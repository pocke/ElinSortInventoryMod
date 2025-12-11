using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace SortInventory;

public static class Settings
{
    public static ConfigEntry<KeyCode> keyCode;
    public static ConfigEntry<KeyCode> keyCodeMod;

    public static ConfigEntry<bool> concatContainers;

    public static ConfigEntry<string> ignoredContainerPattern;

    public static KeyCode KeyCode
    {
        get { return keyCode.Value; }
        set { keyCode.Value = value; }
    }

    public static KeyCode KeyCodeMod
    {
        get { return keyCodeMod.Value; }
        set { keyCodeMod.Value = value; }
    }

    public static bool ConcatContainers
    {
        get { return concatContainers.Value; }
        set { concatContainers.Value = value; }
    }

    public static List<string> IgnoredContainerPattern
    {
        get { return ignoredContainerPattern.Value.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList(); }
        set { ignoredContainerPattern.Value = string.Join(",", value); }
    }
}
