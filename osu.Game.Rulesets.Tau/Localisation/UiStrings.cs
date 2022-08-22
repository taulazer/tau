using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Tau.Localisation
{
    public class UiStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Tau.Localisation.Translations.UI";

        /// <summary>
        /// "Ticks"
        /// </summary>
        public static LocalisableString Ticks => new TranslatableString(getKey(@"ticks"), @"Ticks");

        /// <summary>
        /// "Timing Distribution"
        /// </summary>
        public static LocalisableString TimingDistribution => new TranslatableString(getKey(@"timing_distribution"), @"Timing Distribution");

        /// <summary>
        /// "Paddle Distribution"
        /// </summary>
        public static LocalisableString PaddleDistribution => new TranslatableString(getKey(@"paddle_distribution"), @"Paddle Distribution");

        /// <summary>
        /// "Sliders"
        /// </summary>
        public static LocalisableString Sliders => new TranslatableString(getKey(@"distribution_sliders"), @"Sliders");

        /// <summary>
        /// "Beats"
        /// </summary>
        public static LocalisableString Beats => new TranslatableString(getKey(@"distribution_beats"), @"Beats");

        /// <summary>
        /// "Move the cursor to the highlighted area."
        /// </summary>
        public static LocalisableString ResumeMessage => new TranslatableString(getKey(@"resume_overlay_message"), @"Move the cursor to the highlighted area.");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
