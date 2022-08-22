using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModLite : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Lite";
        public override string Acronym => "LT";
        public override double ScoreMultiplier => 1.0;
        public override IconUsage? Icon => FontAwesome.Solid.History;
        public override LocalisableString Description => ModStrings.LiteDescription;
        public override ModType Type => ModType.Conversion;

        [SettingSource(typeof(ModStrings), nameof(ModStrings.LiteToggleSlidersName), nameof(ModStrings.LiteToggleSlidersDescription))]
        public Bindable<bool> ToggleSliders { get; } = new Bindable<bool>(false);

        [SettingSource(typeof(ModStrings), nameof(ModStrings.LiteToggleHardBeatsName), nameof(ModStrings.LiteToggleHardBeatsDescription))]
        public Bindable<bool> ToggleHardBeats { get; } = new Bindable<bool>(false);

        // maybe replace this with `BeatDivisorControl`?
        [SettingSource(typeof(ModStrings), nameof(ModStrings.LiteSliderDivisionLevelName), nameof(ModStrings.LiteSliderDivisionLevelDescription))]
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
