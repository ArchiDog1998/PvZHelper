using HarmonyLib;

namespace PvZHelper.Patches;

[HarmonyPatch(typeof(CreatePlant), "LimTravel")]
public class CreatePlantPatch
{
    [HarmonyPrefix]
    public static bool LimTravelPost(ref bool __result)
    {
        if (PvZHelperPlugin.UnlimitedAllPlants?.Value ?? false)
        {
            __result = false;
            return false;
        }
        return true;
    }
}
