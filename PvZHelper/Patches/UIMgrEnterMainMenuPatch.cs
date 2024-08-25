using HarmonyLib;

namespace PvZHelper.Patches;

[HarmonyPatch(typeof(UIMgr), "MenuNormalSettings")]

internal class UIMgrEnterMainMenuPatch
{
    [HarmonyPrefix, HarmonyPatch()]
    static void Postfix()
    {
        GameAPP.gameSpeed = 1;
    }
}