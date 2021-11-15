using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModLite : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Lite";
        public override string Acronym => "LT";
        public override double ScoreMultiplier => 1.0;
        public override IconUsage? Icon => FontAwesome.Solid.History;
        public override string Description => "Removes certain aspects of the game.";
        public override ModType Type => ModType.Conversion;

        [SettingSource("No sliders conversion", "Completely disables sliders altogether.")]
        public Bindable<bool> ToggleSliders { get; } = new Bindable<bool>(true);

        [SettingSource("No hard beats conversion", "Completely disables hard beats altogether.")]
        public Bindable<bool> ToggleHardBeats { get; } = new Bindable<bool>(true);

        [SettingSource("Slider division level", "The minimum slider length divisor.")]
        public BindableBeatDivisor SlidersDivisionLevel { get; } = new BindableBeatDivisor
        {
            Default = 2
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var converter = (TauBeatmapConverter)beatmapConverter;

            converter.CanConvertToSliders = !ToggleSliders.Value;
            converter.CanConvertToHardBeats = !ToggleHardBeats.Value;
            converter.SliderDivisionLevel = SlidersDivisionLevel.Value;
        }
    }
}
