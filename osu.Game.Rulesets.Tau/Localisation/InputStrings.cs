using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Tau.Localisation
{
    public class InputStrings
    {
        private const string prefix = @"osu.Game.Rulesets.Tau.Localisation.Translations.Inputs";

        /// <summary>
        /// "Left Button"
        /// </summary>
        public static LocalisableString LeftButton => new TranslatableString(getKey(@"left_button"), @"Left Button");

        /// <summary>
        /// "Right Button"
        /// </summary>
        public static LocalisableString RightButton => new TranslatableString(getKey(@"right_button"), @"Right Button");

        /// <summary>
        /// "Hard Button 1"
        /// </summary>
        public static LocalisableString HardButton1 => new TranslatableString(getKey(@"hard_button_1"), @"Hard Button 1");

        /// <summary>
        /// "Hard Button 2"
        /// </summary>
        public static LocalisableString HardButton2 => new TranslatableString(getKey(@"hard_button_2"), @"Hard Button 2");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
