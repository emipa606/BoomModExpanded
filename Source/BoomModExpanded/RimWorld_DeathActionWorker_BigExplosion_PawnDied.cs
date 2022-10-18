using HarmonyLib;
using RimWorld;
using Verse;

namespace BoomModExpanded;

[HarmonyPatch(typeof(DeathActionWorker_BigExplosion), nameof(DeathActionWorker_BigExplosion.PawnDied),
    typeof(Corpse))]
internal static class RimWorld_DeathActionWorker_BigExplosion_PawnDied
{
    private static bool Prefix(Corpse corpse)
    {
        return corpse.InnerPawn == null || !Evaluator.IsListedPawnKind(corpse.InnerPawn) ||
               Evaluator.IsExplosionImmiment();
    }
}