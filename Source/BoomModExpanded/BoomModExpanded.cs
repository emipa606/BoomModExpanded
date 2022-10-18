using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace BoomModExpanded;

[StaticConstructorOnStartup]
public class BoomModExpanded
{
    static BoomModExpanded()
    {
        Evaluator.UpdateExploders();
        var harmony = new Harmony("Mlie.BoomModExpanded");
        harmony.PatchAll();

        var listOfExplosionPatches = new List<string>
        {
            "RimWorld.DeathActionWorker_AntigrainExplosion",
            "GeneticRim.DeathActionWorker_BiggerExplosion",
            "GeneticRim.DeathActionWorker_EMPExplosion",
            "GeneticRim.DeathActionWorker_Eggxplosion",
            "GeneticRim.DeathActionWorker_FrostExplosion",
            "GeneticRim.DeathActionWorker_GargantuanExplosion",
            "GeneticRim.DeathActionWorker_HairballExplosion",
            "GeneticRim.DeathActionWorker_PsionicExplosion",
            "GeneticRim.DeathActionWorker_SmallBomb",
            "GeneticRim.DeathActionWorker_SmallHairballExplosion",
            "GeneticRim.DeathActionWorker_StunningExplosion",
            "GeneticRim.DeathActionWorker_ToxicExplosion",
            "MorrowRim.DeathActionWorker_RetchingNetch",
            "AlphaBehavioursAndEvents.DeathActionWorker_AcidExplosion",
            "AlphaBehavioursAndEvents.DeathActionWorker_ExplodeAndSpawnEggs",
            "AlphaBehavioursAndEvents.DeathActionWorker_GargantuanExplosion",
            "AlphaBehavioursAndEvents.DeathActionWorker_LuciferiumExplosion",
            "AlphaBehavioursAndEvents.DeathActionWorker_MouseFission",
            "AlphaBehavioursAndEvents.DeathActionWorker_RedAcidExplosion",
            "AlphaBehavioursAndEvents.DeathActionWorker_SmallRedAcidExplosion",
            "AlphaBehavioursAndEvents.DeathActionWorker_SummonFlashstorm"
        };

        foreach (var explosionPatch in listOfExplosionPatches)
        {
            var type = AccessTools.TypeByName(explosionPatch);
            if (type == null)
            {
                continue;
            }

            var method = type.GetMethod("PawnDied");
            if (method == null)
            {
                continue;
            }

            harmony.Patch(method, new HarmonyMethod(typeof(BoomModExpanded).GetMethod(nameof(GenericPatch))));
        }
    }

    public static bool GenericPatch(Corpse corpse)
    {
        return corpse.InnerPawn == null || !Evaluator.IsListedPawnKind(corpse.InnerPawn) ||
               Evaluator.IsExplosionImmiment();
    }
}