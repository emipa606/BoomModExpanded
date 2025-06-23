using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BoomModExpanded;

internal static class Evaluator
{
    private static readonly Dictionary<HediffDef, float> explosionChance = new()
    {
        { HediffDef.Named("Burn"), 1f },
        { HediffDef.Named("Gunshot"), 1f },
        { HediffDef.Named("Shredded"), 1f }
    };

    private static List<string> listedPawnKindDefs;

    private static bool explosionImminent;

    public static Pawn CurrentButcheredPawn;

    public static bool IsListedPawnKind(Pawn pawn)
    {
        return listedPawnKindDefs.Contains(pawn.kindDef.defName);
    }

    public static void UpdateExploders()
    {
        var explodersLoaded = from exploder in DefDatabase<ThingDef>.AllDefsListForReading
            where exploder.race is { deathAction.workerClass: not null } &&
                  exploder.race.deathAction.workerClass.Name.EndsWith("Explosion")
            select exploder;
        listedPawnKindDefs = [];
        var exploderNames = new List<string>();
        foreach (var exploder in explodersLoaded)
        {
            listedPawnKindDefs.Add(exploder.defName);
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

        explosionImminent = true;
        return true;
    }

    public static bool IsExplosive(HediffDef def, Pawn victim)
    {
        if (!BoomModExpandedMod.Instance.Settings.Slaughter && victim == CurrentButcheredPawn)
        {
            return false;
        }

        if (!BoomModExpandedMod.Instance.Settings.Medieval)
        {
            return explosionChance.ContainsKey(def) && Rand.Chance(explosionChance[def]);
        }

        return true;
    }

    public static bool IsExplosionImminent()
    {
        var explode = explosionImminent;
        if (explode)
        {
            explosionImminent = false;
        }

        return explode;
    }
}