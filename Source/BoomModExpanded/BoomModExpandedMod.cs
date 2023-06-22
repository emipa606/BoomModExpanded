using Mlie;
using UnityEngine;
using Verse;

namespace BoomModExpanded;

[StaticConstructorOnStartup]
internal class BoomModExpandedMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static BoomModExpandedMod instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    public readonly BoomModExpandedSettings Settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public BoomModExpandedMod(ModContentPack content) : base(content)
    {
        instance = this;
        Settings = GetSettings<BoomModExpandedSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Boom Mod Expanded";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.Label("BME.explodefor.label".Translate(), -1,
            "BME.explodefor.tooltip".Translate());
        listing_Standard.CheckboxLabeled("BME.slaughter.label".Translate(), ref Settings.Slaughter,
            "BME.slaughter.tooltip".Translate());
        listing_Standard.CheckboxLabeled("BME.medieval.label".Translate(), ref Settings.Medival,
            "BME.medieval.tooltip".Translate());
        if (Settings.Slaughter && Settings.Medival)
        {
            listing_Standard.Label("BME.nothing.label".Translate());
        }

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("BME.version.label".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        //listing_Standard.CheckboxLabeled("Natural causes", ref Settings.Naturally, "Don't explode when dying naturally");
        listing_Standard.End();
        Settings.Write();
    }
}