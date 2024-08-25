using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace PvZHelper.Patches;

[HarmonyPatch(typeof(Mouse), "TryToSetPlantByCard")]
internal class MouseSetPlantPatch
{
    static readonly FieldInfo boardType = AccessTools.Field(typeof(GameAPP), nameof(GameAPP.theBoardType));
    static readonly FieldInfo boardLevel = AccessTools.Field(typeof(GameAPP), nameof(GameAPP.theBoardLevel));
    static readonly MethodInfo myBoardType = AccessTools.PropertyGetter(typeof(MouseSetPlantPatch), nameof(BoardType));
    static readonly MethodInfo myBoardLevel = AccessTools.PropertyGetter(typeof(MouseSetPlantPatch), nameof(BoardLevel));
    public static int BoardType => (PvZHelperPlugin.Columns?.Value ?? false) ? 1 : GameAPP.theBoardType;
    public static int BoardLevel => (PvZHelperPlugin.Columns?.Value ?? false) ? 38 : GameAPP.theBoardLevel;

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.LoadsField(boardType))
            {
                yield return new CodeInstruction(OpCodes.Call, myBoardType);
            }
            else if (instruction.LoadsField(boardLevel))
            {
                yield return new CodeInstruction(OpCodes.Call, myBoardLevel);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
