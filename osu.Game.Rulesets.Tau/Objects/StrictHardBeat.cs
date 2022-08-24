using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Objects
{
    public class StrictHardBeat : AngledTauHitObject
    {
        public double Range { get; private set; }

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            // TODO: maybe this should be a static method instead?
            var properties = new TauCachedProperties();
            properties.SetRange(difficulty.CircleSize);

            double multiplier = IBeatmapDifficultyInfo.DifficultyRange(difficulty.OverallDifficulty, 0.1, 0.25, 0.5);

            Range = properties.AngleRange.Value * multiplier;
        }
    }
}
