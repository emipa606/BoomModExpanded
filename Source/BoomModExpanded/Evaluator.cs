using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BoomModExpanded;

internal static class Evaluator
{
    private static readonly Dictionary<HediffDef, float> ExplosionChance = new Dictionary<HediffDef, float>
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
        var explodersLoaded = from exploder in DefDatabase<ThingDef>.AllDefsListForReading
            where exploder.race is { deathActionWorkerClass: { } } &&
                  exploder.race.deathActionWorkerClass.Name.EndsWith("Explosion")
            select exploder;
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
        if (cause == null)
        {
            return false;
        }

        var def = (cause as Hediff_MissingPart)?.lastInjury ?? cause.def;

        if (!pawn.HasAttachment(ThingDefOf.Fire) && !IsExplosive(def, pawn))
        {
            return false;
        }

        _explosionImminent = true;
        return true;
    }

    private static bool IsExplosive(HediffDef def, Pawn victim)
    {
        //Log.Message($"Checking if {victim.NameShortColored} should explode of {def.label}");
        if (!BoomModExpandedMod.instance.Settings.Slaughter && victim == currentButcheredPawn)
        {
            //Log.Message($"No, {victim.NameShortColored} is being slaughtered");
            return false;
        }

        if (!BoomModExpandedMod.instance.Settings.Medival)
        {
            //Log.Message($"Only if {def.label} is a modern wound");
            return ExplosionChance.ContainsKey(def) && Rand.Chance(ExplosionChance[def]);
        }

        //Log.Message("Yes");
        return true;
    }

    public static bool IsExplosionImmiment()
    {
        var explode = _explosionImminent;
        if (explode)
        {
            _explosionImminent = false;
        }

        return explode;
    }
}