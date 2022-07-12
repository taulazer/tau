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

        protected override DifficultyRange[] GetRanges() => new[]
        {
            new DifficultyRange(HitResult.Great, 64, 49, 34),
            new DifficultyRange(HitResult.Ok, 127, 112, 97),
            new DifficultyRange(HitResult.Miss, 127, 112, 97),
        };
    }
}
