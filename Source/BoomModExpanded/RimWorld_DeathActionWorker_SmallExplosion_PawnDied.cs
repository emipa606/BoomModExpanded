using HarmonyLib;
using RimWorld;
using Verse;

namespace BoomModExpanded;

[HarmonyPatch(typeof(DeathActionWorker_SmallExplosion), nameof(DeathActionWorker_SmallExplosion.PawnDied),
    typeof(Corpse))]
internal static class RimWorld_DeathActionWorker_SmallExplosion_PawnDied
{
    private static bool Prefix(Corpse corpse)
    {
        return corpse.InnerPawn == null || !Evaluator.IsListedPawnKind(corpse.InnerPawn) ||
               Evaluator.IsExplosionImmiment();
    }
}