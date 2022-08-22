using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Tau.Localisation
{
    public class ModStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Tau.Localisation.Translations.Mods";

        #region Autopilot

        /// <summary>
        /// "Automatic paddle movement - just follow the rhythm."
        /// </summary>
        public static LocalisableString AutopilotDescription
            => new TranslatableString(getKey(@"autopilot_description"), @"Automatic paddle movement - just follow the rhythm.");

        #endregion

        #region Hidden

        /// <summary>
        /// "Beats fade out before you hit them!"
        /// </summary>
        public static LocalisableString FadeOutDescription
            => new TranslatableString(getKey(@"fadeout_description"), @"Beats fade out before you hit them!");

        /// <summary>
        /// "Beats appear out of nowhere!"
        /// </summary>
        public static LocalisableString FadeInDescription
            => new TranslatableString(getKey(@"fadein_description"), @"Beats appear out of nowhere!");

        #endregion

        #region Inverse

        /// <summary>
        /// "Beats will appear outside of the playfield."
        /// </summary>
        public static LocalisableString InverseDescription
            => new TranslatableString(getKey(@"inverse_description"), @"Beats will appear outside of the playfield.");

        #endregion

        #region Impossible Sliders

        /// <summary>
        /// "Entirely removes the check for very sharp angles"
        /// </summary>
        public static LocalisableString ImpossibleSlidersDescription
            => new TranslatableString(getKey(@"impossible_sliders_description"), @"Entirely removes the check for very sharp angles");

        #endregion

        #region Lite

        /// <summary>
        /// "Removes certain aspects of the ruleset."
        /// </summary>
        public static LocalisableString LiteDescription
            => new TranslatableString(getKey(@"lite_description"), @"Removes certain aspects of the ruleset.");

        /// <summary>
        /// "Sliders conversion"
        /// </summary>
        public static LocalisableString LiteToggleSlidersName
            => new TranslatableString(getKey(@"lite_toggle_sliders_name"), @"Sliders conversion");

        /// <summary>
        /// "Completely disables sliders altogether."
        /// </summary>
        public static LocalisableString LiteToggleSlidersDescription
            => new TranslatableString(getKey(@"lite_toggle_sliders_description"), @"Completely disables sliders altogether.");

        /// <summary>
        /// "Hard beats conversion"
        /// </summary>
        public static LocalisableString LiteToggleHardBeatsName
            => new TranslatableString(getKey(@"lite_toggle_hard_beats_name"), @"Hard beats conversion");

        /// <summary>
        /// "Completely disables hard beats altogether."
        /// </summary>
        public static LocalisableString LiteToggleHardBeatsDescription
            => new TranslatableString(getKey(@"lite_toggle_hard_beats_description"), @"Completely disables hard beats altogether.");

        /// <summary>
        /// "Slider division level"
        /// </summary>
        public static LocalisableString LiteSliderDivisionLevelName
            => new TranslatableString(getKey(@"lite_slider_division_level_name"), @"Slider division level");

        /// <summary>
        /// "The minimum slider length divisor."
        /// </summary>
        public static LocalisableString LiteSliderDivisionLevelDescription
            => new TranslatableString(getKey(@"lite_slider_division_level_description"), @"The minimum slider length divisor.");

        #endregion

        #region No Scope

        /// <summary>
        /// "Where's the paddle?"
        /// </summary>
        public static LocalisableString NoScopeDescription
            => new TranslatableString(getKey(@"no_scope_description"), @"Where's the paddle?");

        /// <summary>
        /// "Hidden at combo"
        /// </summary>
        public static LocalisableString NoScopeThresholdName
            => new TranslatableString(getKey(@"no_scope_threshold_name"), @"Hidden at combo");

        /// <summary>
        /// "The combo count at which the paddle becomes completely hidden"
        /// </summary>
        public static LocalisableString NoScopeThresholdDescription
            => new TranslatableString(getKey(@"no_scope_threshold_description"), @"The combo count at which the paddle becomes completely hidden");

        #endregion

        #region Relax

        /// <summary>
        /// "You don't need to click. Give your clicking/tapping fingers a break from the heat of things."
        /// </summary>
        public static LocalisableString RelaxDescription
            => new TranslatableString(getKey(@"relax_description"), @"You don't need to click. Give your clicking/tapping fingers a break from the heat of things.");

        #endregion

        #region Roundabout

        /// <summary>
        /// "You can only rotate the paddle in one direction."
        /// </summary>
        public static LocalisableString RoundaboutDescription
            => new TranslatableString(getKey(@"roundabout_description"), @"You can only rotate the paddle in one direction.");

        /// <summary>
        /// "Direction"
        /// </summary>
        public static LocalisableString RoundaboutDirectionName
            => new TranslatableString(getKey(@"roundabout_direction_name"), @"Direction");

        #endregion

        #region Traceable

        /// <summary>
        /// "Brim with no yankie"
        /// </summary>
        public static LocalisableString TraceableDescription
            => new TranslatableString(getKey(@"traceable_description"), @"Brim with no yankie");

        #endregion

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
