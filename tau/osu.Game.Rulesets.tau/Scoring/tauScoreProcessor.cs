// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Scoring
{
    public class TauScoreProcessor : ScoreProcessor<TauHitObject>
    {
        public TauScoreProcessor(DrawableRuleset<TauHitObject> ruleset)
            : base(ruleset)
        {
        }

        private float hpDrainRate;

        protected override void ApplyBeatmap(Beatmap<TauHitObject> beatmap)
        {
            base.ApplyBeatmap(beatmap);

            hpDrainRate = beatmap.BeatmapInfo.BaseDifficulty.DrainRate;
        }

        protected override double HealthAdjustmentFactorFor(JudgementResult result)
        {
            switch (result.Type)
            {
                case HitResult.Miss:
                    return hpDrainRate;

                default:
                    return 10.2 - hpDrainRate; // Award less HP as drain rate is increased
            }
        }

        public override HitWindows CreateHitWindows() => new TauHitWindows();
    }
}
