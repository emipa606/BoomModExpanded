using UnityEngine;
using Verse;

namespace BoomModExpanded
{
    [StaticConstructorOnStartup]
    internal class BoomModExpandedMod : Mod
    {
        /// <summary>
        /// Cunstructor
        /// </summary>
        /// <param name="content"></param>
        public BoomModExpandedMod(ModContentPack content) : base(content)
        {
            instance = this;
        }

        /// <summary>
        /// The instance-settings for the mod
        /// </summary>
        internal BoomModExpandedSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<BoomModExpandedSettings>();
                }
                return settings;
            }
            set => settings = value;
        }

        /// <summary>
        /// The title for the mod-settings
        /// </summary>
        /// <returns></returns>
        public override string SettingsCategory()
        {
            return "Boom Mod Expanded";
        }

        /// <summary>
        /// The settings-window
        /// For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
        /// </summary>
        /// <param name="rect"></param>
        public override void DoSettingsWindowContents(Rect rect)
        {
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            listing_Standard.Gap();
            listing_Standard.Label("Explode for", -1, "The following events are included in causing explosions of exploding pawns.");
            listing_Standard.CheckboxLabeled("Slaughter", ref Settings.Slaughter, "Explode when slaughtering");
            listing_Standard.CheckboxLabeled("Melee or medival", ref Settings.Medival, "Explode when dying of a melee or ranged medival");
            if(Settings.Slaughter && Settings.Medival)
            {
                listing_Standard.Label("Well, this is awkward. This mod will not do anything if you dont unselect any of the options...");
            }
            //listing_Standard.CheckboxLabeled("Natural causes", ref Settings.Naturally, "Dont explode when dying naturally");
            listing_Standard.End();
            Settings.Write();
        }


        /// <summary>
        /// The instance of the settings to be read by the mod
        /// </summary>
        public static BoomModExpandedMod instance;

        /// <summary>
        /// The private settings
        /// </summary>
        private BoomModExpandedSettings settings;

    }
}
