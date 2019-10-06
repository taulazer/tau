// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.tau.Objects
{
    public class TauHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new Judgement();

        public double TimePreempt = 600;
        public double TimeFadeIn = 100;

        public float Angle { get; set; }
        public Vector2 PositionToEnd { get; set; }

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimePreempt = (float)BeatmapDifficulty.DifficultyRange(difficulty.ApproachRate, 1800, 1200, 450);
            TimeFadeIn = 100;
        }
    }
}
