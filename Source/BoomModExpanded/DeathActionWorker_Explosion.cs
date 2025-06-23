using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BoomModExpanded;

[HarmonyPatch]
internal static class DeathActionWorker_Explosion
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(DeathActionWorker_BigExplosion),
            nameof(DeathActionWorker_BigExplosion.PawnDied));
        yield return AccessTools.Method(typeof(DeathActionWorker_SmallExplosion),
            nameof(DeathActionWorker_SmallExplosion.PawnDied));

        if (ModLister.GetActiveModWithIdentifier("BiomesTeam.BiomesCaverns", true) == null)
        {
            yield break;
        }

        Log.Message("[BoomModExpanded]: Adding support for Biomes! Caverns");
        yield return AccessTools.Method("BiomesCaverns.DeathActionWorkers.DeathActionWorker_SharpExplosion:PawnDied");
    }

    public static bool Prefix(Corpse corpse)
    {
        if (corpse.InnerPawn == null)
        {
            return true;
        }

        return !Evaluator.IsListedPawnKind(corpse.InnerPawn) || Evaluator.IsExplosionImminent();
    }
}