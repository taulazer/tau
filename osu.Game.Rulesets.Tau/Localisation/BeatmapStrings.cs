using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Tau.Localisation
{
    public class BeatmapStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Tau.Localisation.Translations.Beatmap";

        /// <summary>
        /// "Beat count"
        /// </summary>
        public static LocalisableString BeatCount => new TranslatableString(getKey(@"beat_count"), @"Beat count");

        /// <summary>
        /// "Slider count"
        /// </summary>
        public static LocalisableString SliderCount => new TranslatableString(getKey(@"slider_count"), @"Slider count");

        /// <summary>
        /// "Hard Beat count"
        /// </summary>
        public static LocalisableString HardBeatCount => new TranslatableString(getKey(@"hard_beat_count"), @"Hard Beat count");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
