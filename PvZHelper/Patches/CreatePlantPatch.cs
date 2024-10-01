using HarmonyLib;

namespace PvZHelper.Patches;

[HarmonyPatch(typeof(CreatePlant), "LimTravel")]
public class CreatePlantPatch
{
    [HarmonyPrefix]
    public static bool LimTravelPost(ref bool __result)
    {
        return __result = false;
    }
}
