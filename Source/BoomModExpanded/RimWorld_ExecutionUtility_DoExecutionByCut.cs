using HarmonyLib;
using RimWorld;
using Verse;

namespace BoomModExpanded
{
    [HarmonyPatch(typeof(ExecutionUtility), nameof(ExecutionUtility.DoExecutionByCut))]
    internal static class RimWorld_ExecutionUtility_DoExecutionByCut
    {
        private static void Prefix(Pawn victim)
        {
            Evaluator.currentButcheredPawn = victim;
        }
    }
}