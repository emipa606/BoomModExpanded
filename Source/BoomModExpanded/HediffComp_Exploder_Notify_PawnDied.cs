using Verse;

namespace BoomModExpanded;

internal static class HediffComp_Exploder_Notify_PawnDied
{
    public static bool Prefix(HediffComp __instance, Hediff culprit)
    {
        if (__instance.parent.pawn == null)
        {
            return true;
        }

        return culprit == null || Evaluator.IsExplosive(culprit.def, __instance.parent.pawn);
    }
}