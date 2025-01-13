using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BoomModExpanded;

internal static class Evaluator
{
    private static readonly Dictionary<HediffDef, float> ExplosionChance = new Dictionary<HediffDef, float>
    {
        { HediffDef.Named("Burn"), 1f },
        { HediffDef.Named("Gunshot"), 1f },
        { HediffDef.Named("Shredded"), 1f }
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
            where exploder.race is { deathAction.workerClass: not null } &&
                  exploder.race.deathAction.workerClass.Name.EndsWith("Explosion")
            select exploder;
        ListedPawnKindDefs = [];
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

    public static bool IsExplosive(HediffDef def, Pawn victim)
    {
        if (!BoomModExpandedMod.instance.Settings.Slaughter && victim == currentButcheredPawn)
        {
            return false;
        }

        if (!BoomModExpandedMod.instance.Settings.Medival)
        {
            return ExplosionChance.ContainsKey(def) && Rand.Chance(ExplosionChance[def]);
        }

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