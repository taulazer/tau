using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModDual : Mod
    {
        public override string Name => "Dual";
        public override string Acronym => "DL";
        public override LocalisableString Description => ModStrings.DualDescription;
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.Fun;

        public override bool HasImplementation => true;

        [SettingSource(typeof(ModStrings), nameof(ModStrings.DualPaddleCountName))]
        public BindableNumber<int> PaddleCount { get; } = new()
        {
            Value = 2,
            Default = 2,
            MinValue = 2,
            MaxValue = 4
        };
    }
}
