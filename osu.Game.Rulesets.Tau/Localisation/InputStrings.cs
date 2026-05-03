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

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
