// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Scoring
{
    public class TauHitWindows : HitWindows
    {
        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                case HitResult.Good:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }

        protected override DifficultyRange[] GetRanges() => new[]
        {
            new DifficultyRange(HitResult.Great, 199, 199, 199),
            new DifficultyRange(HitResult.Good, 299, 250, 200),
            new DifficultyRange(HitResult.Miss, 300, 300, 300),
        };
    }
}
