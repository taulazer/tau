using System;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing
{
    public class TauAngledDifficultyHitObject : TauDifficultyHitObject
    {
        /// <summary>
        /// Normalised distance between the start and end position of this <see cref="TauAngledDifficultyHitObject"/>.
        /// </summary>
        public double TravelDistance { get; private set; }

        public double LazyTravelDistance { get; private set; }

        /// <summary>
        /// The time taken to travel through <see cref="TravelDistance"/>, with a minimum value of 25ms for a non-zero distance.
        /// </summary>
        public double TravelTime { get; private set; }

        private readonly TauCachedProperties properties;

        public double AngleRange => properties.AngleRange.Value;

        public readonly double Distance;

        public TauAngledDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, TauCachedProperties properties)
            : base(hitObject, lastObject, clockRate)
        {
            this.properties = properties;

            if (hitObject is AngledTauHitObject firstAngled && lastObject is AngledTauHitObject lastAngled)
            {
                float offset = 0;

                if (lastAngled is IHasOffsetAngle offsetAngle)
                    offset = offsetAngle.GetOffsetAngle();

                Distance = Math.Abs(Extensions.GetDeltaAngle(firstAngled.Angle, (lastAngled.Angle + offset).Normalize()));
            }

            if (hitObject is Slider slider)
            {
                TravelDistance = slider.Path.CalculatedDistance;
                LazyTravelDistance = slider.Path.CalculateLazyDistance((float)(AngleRange / 2));
                TravelTime = slider.Duration;
            }
        }
    }
}
