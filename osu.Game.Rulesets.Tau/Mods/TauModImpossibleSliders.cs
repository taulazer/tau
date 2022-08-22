using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModImpossibleSliders : Mod, IApplicableToBeatmapConverter
    {
        public override string Name => "Impossible Sliders";
        public override LocalisableString Description => ModStrings.ImpossibleSlidersDescription;
        public override double ScoreMultiplier => 1f;
        public override string Acronym => "IS";

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            var converter = (TauBeatmapConverter)beatmapConverter;

            converter.CanConvertImpossibleSliders = true;
        }
    }
}
