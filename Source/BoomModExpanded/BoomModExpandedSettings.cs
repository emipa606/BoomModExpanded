using Verse;

namespace BoomModExpanded;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class BoomModExpandedSettings : ModSettings
{
    public bool Medival;

    public bool Slaughter;
    //public bool Naturally = true;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Slaughter, "Slaughter");
        Scribe_Values.Look(ref Medival, "Medival");
        //Scribe_Values.Look(ref Naturally, "Naturally", true, false);
    }
}