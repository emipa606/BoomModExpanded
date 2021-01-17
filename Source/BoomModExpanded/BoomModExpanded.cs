using Verse;
using HarmonyLib;
using System.Collections.Generic;
using RimWorld;
using System.Linq;

namespace BoomModExpanded
{
    [StaticConstructorOnStartup]
    public class BoomModExpanded
    {
        static BoomModExpanded()
        {
            Evaluator.UpdateExploders();
            new Harmony("Mlie.BoomModExpanded").PatchAll();
        }
    }

    internal static class Evaluator
    {
        public static Dictionary<HediffDef, float> ExplosionChance = new Dictionary<HediffDef, float>
        {
            { HediffDefOf.Burn, 1f },
            { HediffDefOf.Gunshot, 1f },
            { HediffDefOf.Shredded, 1f }
        };

        private static List<string> ListedPawnKindDefs;

        private static bool _explosionImminent;

        public static Pawn currentButcheredPawn;

        public static bool IsListedPawnKind(Pawn pawn)
        {
            return ListedPawnKindDefs.Contains(pawn.kindDef.defName);
        }

        public static void UpdateExploders()
        {
            var explodersLoaded = from exploder in DefDatabase<ThingDef>.AllDefsListForReading where exploder.race != null && exploder.race.deathActionWorkerClass != null && exploder.race.deathActionWorkerClass.Name.EndsWith("Explosion") select exploder;
            ListedPawnKindDefs = new List<string>();
            var exploderNames = new List<string>();
            foreach (var exploder in explodersLoaded)
            {
                ListedPawnKindDefs.Add(exploder.defName);
                exploderNames.Add(exploder.label);
            }
            Log.Message($"BoomModExpanded: Current exploders are {string.Join(", ", exploderNames)}");
        }

        public static bool IsExploding(Pawn pawn, Hediff cause)
        {
            if (cause == null) { return false; }

            var def = (cause as Hediff_MissingPart)?.lastInjury ?? cause.def;

            if (pawn.HasAttachment(ThingDefOf.Fire) || IsExplosive(def, pawn))
            {
                _explosionImminent = true;
                return true;
            }
            return false;
        }

        private static bool IsExplosive(HediffDef def, Pawn victim)
        {
            //Log.Message($"Checking if {victim.NameShortColored} should explode of {def.label}");
            if (!BoomModExpandedMod.instance.Settings.Slaughter && victim == Evaluator.currentButcheredPawn)
            {
                //Log.Message($"No, {victim.NameShortColored} is being slaughtered");
                return false;
            }
            if (!BoomModExpandedMod.instance.Settings.Medival)
            {
                //Log.Message($"Only if {def.label} is a modern wound");
                return ExplosionChance.ContainsKey(def) && Rand.Chance(ExplosionChance[def]);
            }
            //Log.Message($"Yes");
            return true;
        }

        public static bool IsExplosionImmiment()
        {
            var explode = _explosionImminent;
            if (explode) { _explosionImminent = false; }

            return explode;
        }
    }

    [HarmonyPatch(typeof(DeathActionWorker_BigExplosion), nameof(DeathActionWorker_BigExplosion.PawnDied), typeof(Corpse))]
    internal static class RimWorld_DeathActionWorker_BigExplosion_PawnDied
    {
        private static bool Prefix(Corpse corpse)
        {
            return (corpse.InnerPawn == null) || !Evaluator.IsListedPawnKind(corpse.InnerPawn) || Evaluator.IsExplosionImmiment();
        }
    }

    [HarmonyPatch(typeof(ExecutionUtility), nameof(ExecutionUtility.DoExecutionByCut), new[] { typeof(Pawn), typeof(Pawn) })]
    internal static class RimWorld_ExecutionUtility_DoExecutionByCut
    {
        private static void Prefix(Pawn executioner, Pawn victim)
        {
            Evaluator.currentButcheredPawn = victim;
        }
    }

    [HarmonyPatch(typeof(DeathActionWorker_SmallExplosion), nameof(DeathActionWorker_SmallExplosion.PawnDied), typeof(Corpse))]
    internal static class RimWorld_DeathActionWorker_SmallExplosion_PawnDied
    {
        private static bool Prefix(Corpse corpse)
        {
            return (corpse.InnerPawn == null) || !Evaluator.IsListedPawnKind(corpse.InnerPawn) || Evaluator.IsExplosionImmiment();
        }
    }

    [HarmonyPatch(typeof(BattleLogEntry_StateTransition), MethodType.Constructor, typeof(Thing), typeof(RulePackDef), typeof(Pawn), typeof(Hediff), typeof(BodyPartRecord))]
    internal static class Verse_BattleLogEntry_StateTransition
    {
        private static void Prefix(Thing subject, ref RulePackDef transitionDef, Hediff culpritHediff)
        {
            if (!(subject is Pawn pawn) || !Evaluator.IsListedPawnKind(pawn) || Evaluator.IsExploding(pawn, culpritHediff)) { return; }
            transitionDef = RulePackDefOf.Transition_Died;
        }
    }
}
