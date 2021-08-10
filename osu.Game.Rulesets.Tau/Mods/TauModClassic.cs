using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModClassic : ModClassic, IApplicableToBeatmapConverter
    {
        [SettingSource("No sliders conversion", "Completely disables sliders altogether.")]
        public Bindable<bool> ToggleSliders { get; } = new Bindable<bool>(true);

        [SettingSource("No hard beats conversion", "Completely disables hard beats altogether.")]
        public Bindable<bool> ToggleHardBeats { get; } = new Bindable<bool>(true);

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var converter = (TauBeatmapConverter)beatmapConverter;

            converter.CanConvertToSliders = !ToggleSliders.Value;
            converter.CanConvertToHardBeats = !ToggleHardBeats.Value;
        }
    }
}
