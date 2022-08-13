using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModDual : Mod
    {
        public override string Name => "Dual";
        public override string Acronym => "DL";
        public override string Description => "When one isn't enough";
        public override double ScoreMultiplier => 1;
        public override ModType Type => ModType.Fun;

        public override bool HasImplementation => true;

        [SettingSource("Paddle count")]
        public BindableNumber<int> PaddleCount { get; } = new()
        {
            Value = 2,
            Default = 2,
            MinValue = 2,
            MaxValue = 4
        };
    }
}
