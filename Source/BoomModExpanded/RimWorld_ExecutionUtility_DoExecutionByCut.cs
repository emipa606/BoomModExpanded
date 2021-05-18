using HarmonyLib;
using RimWorld;
using Verse;

namespace BoomModExpanded
{
    [HarmonyPatch(typeof(ExecutionUtility), nameof(ExecutionUtility.DoExecutionByCut), typeof(Pawn), typeof(Pawn))]
    internal static class RimWorld_ExecutionUtility_DoExecutionByCut
    {
        private static void Prefix(Pawn victim)
        {
            Evaluator.currentButcheredPawn = victim;
        }
    }
}