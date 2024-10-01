using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using PvZHelper.Behaviors;
using UnityEngine;

namespace PvZHelper;

[BepInPlugin(GUID, "PvZ Helper", "1.0.0")]
public class PvZHelperPlugin : BasePlugin
{
    private readonly Harmony _harmony = new(GUID);
    private const string GUID = "pvz.rh.helper";

    public override void Load()
    {
        _harmony.PatchAll();

        SetupConfigs();
        AddBehaviors();
    }

    private void SetupConfigs()
    {
    }

    private static void AddBehaviors()
    {
        GameObject obj = new("CustomObject")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        UnityEngine.Object.DontDestroyOnLoad(obj);

        AddBehavior<AutoSunBehavior>(obj);
        AddBehavior<PresetBehavior>(obj);

#if DEBUG
        AddBehavior<SeedTypeLoader>(obj);
#endif
    }

    private static void AddBehavior<T>(GameObject obj) where T : Component
    {
        ClassInjector.RegisterTypeInIl2Cpp<T>();
        obj.AddComponent<T>();
    }

    public override bool Unload()
    {
        _harmony.UnpatchSelf();
        return base.Unload();
    }
}
