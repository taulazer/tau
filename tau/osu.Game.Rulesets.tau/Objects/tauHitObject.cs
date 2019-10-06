// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.tau.Objects
{
    public class TauHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new Judgement();

        public double TimePreempt = 600;
        public double TimeFadeIn = 400;

        public float Angle { get; set; }
        public Vector2 PositionToEnd { get; set; }
    }
}
