using System;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModStrict : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Strict";
        public override LocalisableString Description => ModStrings.StrictDescription;
        public override double ScoreMultiplier => 1.2;
        public override string Acronym => "ST";
        public override ModType Type => ModType.DifficultyIncrease;
        public override Type[] IncompatibleMods => new[] { typeof(TauModLenience), typeof(TauModLite) };

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var converter = (TauBeatmapConverter)beatmapConverter;
            converter.HardBeatsAreStrict = true;
        }
    }
}
