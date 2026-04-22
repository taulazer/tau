using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Tau.Scoring
{
    public class TauHitWindow : HitWindows
    {
        public override bool IsHitResultAllowed(HitResult result)
            => result switch
            {
                HitResult.Great
                    or HitResult.Ok
                    or HitResult.Miss
                    or HitResult.SmallTickHit
                    or HitResult.SmallTickMiss => true,
                _ => false
            };

        private readonly DifficultyRange greatWindowRange = new DifficultyRange(64, 49, 34);
        private readonly DifficultyRange okWindowRange = new DifficultyRange(127, 112, 97);

        private double great;
        private double ok;

        public override void SetDifficulty(double difficulty)
        {
            great = IBeatmapDifficultyInfo.DifficultyRange(difficulty, greatWindowRange);
            ok = IBeatmapDifficultyInfo.DifficultyRange(difficulty, okWindowRange);
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    return great;

                case HitResult.Ok:
                case HitResult.Miss:
                    return ok;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}
