using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Tau.Localisation
{
    public static class SettingStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Tau.Localisation.Translations.Settings";

        /// <summary>
        /// "Show Effects"
        /// </summary>
        public static LocalisableString ShowEffects => new TranslatableString(getKey(@"show_effects"), @"Show Effects");

        /// <summary>
        /// "Show Slider Effects"
        /// </summary>
        public static LocalisableString ShowSliderEffects => new TranslatableString(getKey(@"show_slider_effects"), @"Show Slider Effects");

        /// <summary>
        /// "Show Visualizer"
        /// </summary>
        public static LocalisableString ShowVisualizer => new TranslatableString(getKey(@"show_visualizer"), @"Show Visualizer");

        /// <summary>
        /// "Hit Lighting"
        /// </summary>
        public static LocalisableString HitLighting => new TranslatableString(getKey(@"hit_lighting"), @"Hit Lighting");

        /// <summary>
        /// "Kiai Type"
        /// </summary>
        public static LocalisableString KiaiType => new TranslatableString(getKey(@"kiai_type"), @"Kiai Type");

        /// <summary>
        /// "Playfield Dim"
        /// </summary>
        public static LocalisableString PlayfieldDim => new TranslatableString(getKey(@"playfield_dim"), @"Playfield Dim");

        /// <summary>
        /// "Notes Size"
        /// </summary>
        public static LocalisableString NotesSize => new TranslatableString(getKey(@"notes_size"), @"Notes Size");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
