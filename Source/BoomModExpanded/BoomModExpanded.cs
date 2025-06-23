using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace BoomModExpanded;

[StaticConstructorOnStartup]
public class BoomModExpanded
{
    static BoomModExpanded()
    {
        Evaluator.UpdateExploders();
        new Harmony("Mlie.BoomModExpanded").PatchAll(Assembly.GetExecutingAssembly());

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

        MethodInfo method;
        foreach (var explosionPatch in listOfExplosionPatches)
        {
            var type = AccessTools.TypeByName(explosionPatch);
            if (type == null)
            {
                continue;
            }

            method = type.GetMethod("PawnDied");
            if (method == null)
            {
                continue;
            }

            new Harmony("Mlie.BoomModExpanded").Patch(method,
                new HarmonyMethod(typeof(BoomModExpanded).GetMethod(nameof(GenericPatch))));
        }

        if (ModLister.GetActiveModWithIdentifier("OskarPotocki.VanillaFactionsExpanded.Core", true) != null)
        {
            Log.Message("[BoomModExpanded]: Adding support for Animal Behaviour");
            method = AccessTools.Method("AnimalBehaviours.HediffComp_Exploder:Notify_PawnDied");
            new Harmony("Mlie.BoomModExpanded").Patch(method,
                new HarmonyMethod(HediffComp_Exploder_Notify_PawnDied.Prefix));
        }


        if (ModLister.GetActiveModWithIdentifier("sarg.alphagenes", true) == null)
        {
            return;
        }

        Log.Message("[BoomModExpanded]: Adding support for Alpha Genes");
        method = AccessTools.Method("AlphaGenes.HediffComp_Exploder:Notify_PawnDied");
        new Harmony("Mlie.BoomModExpanded").Patch(method,
            new HarmonyMethod(HediffComp_Exploder_Notify_PawnDied.Prefix));
    }

    public static bool GenericPatch(Corpse corpse)
    {
        return corpse.InnerPawn == null || !Evaluator.IsListedPawnKind(corpse.InnerPawn) ||
               Evaluator.IsExplosionImminent();
    }
}