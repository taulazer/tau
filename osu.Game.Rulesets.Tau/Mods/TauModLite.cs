using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;

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

        public override Type[] IncompatibleMods => new[] { typeof(TauModStrict), typeof(TauModLenience) };

        [SettingSource("Sliders conversion", "Completely disables sliders altogether.")]
        public Bindable<bool> ToggleSliders { get; } = new Bindable<bool>(false);

        [SettingSource("Hard beats conversion", "Completely disables hard beats altogether.")]
        public Bindable<bool> ToggleHardBeats { get; } = new Bindable<bool>(false);

        // maybe replace this with `BeatDivisorControl`?
        [SettingSource("Slider division level", "The minimum slider length divisor.")]
        public BindableInt SlidersDivisionLevel { get; } = new BindableInt
        {
            Value = 2,
            Default = 2,
            MinValue = 1,
            MaxValue = 64
        };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var converter = (TauBeatmapConverter)beatmapConverter;

            converter.CanConvertToHardBeats = ToggleHardBeats.Value;
            converter.CanConvertToSliders = ToggleSliders.Value;
            converter.SliderDivisor = SlidersDivisionLevel.Value;
        }
    }
}
