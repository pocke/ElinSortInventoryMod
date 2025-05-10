using BepInEx.Configuration;
using UnityEngine;

namespace SortContainers;

public static class Settings
{
    public static ConfigEntry<KeyCode> keyCode;
    public static ConfigEntry<KeyCode> keyCodeMod;

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
}
