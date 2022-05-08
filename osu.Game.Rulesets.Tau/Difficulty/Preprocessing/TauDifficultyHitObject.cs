using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing
{
    public class TauDifficultyHitObject : DifficultyHitObject
    {
        private readonly TauCachedProperties properties;

        public double AngleRange => properties.AngleRange.Value;

        public TauDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, TauCachedProperties properties)
            : base(hitObject, lastObject, clockRate)
        {
            this.properties = properties;
        }
    }
}
