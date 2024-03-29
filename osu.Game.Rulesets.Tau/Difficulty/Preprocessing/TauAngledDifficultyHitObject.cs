using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Difficulty.Preprocessing;
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
        public double TravelDistance { get; }

        /// <summary>
        /// Normalised distance between the start and end position of this <see cref="TauAngledDifficultyHitObject"/> in the shortest span possible to account for slider cheesing.
        /// </summary>
        public double LazyTravelDistance { get; }

        /// <summary>
        /// The time taken to travel through <see cref="TravelDistance"/>, with a minimum value of 25ms for a non-zero distance.
        /// </summary>
        public double TravelTime { get; }

        private readonly TauCachedProperties properties;

        public new AngledTauHitObject BaseObject => (AngledTauHitObject)base.BaseObject;

        public TauAngledDifficultyHitObject LastAngled;

        public double AngleRange => properties.AngleRange.Value;

        public double Distance;

        public TauAngledDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, List<DifficultyHitObject> objects, int index,
                                            TauCachedProperties properties,
                                            TauAngledDifficultyHitObject lastAngled)
            : base(hitObject, lastObject, clockRate, objects, index)
        {
            this.properties = properties;
            LastAngled = lastAngled;

            if (hitObject is AngledTauHitObject firstAngled && lastAngled != null)
            {
                float offset = 0;

                if (lastAngled.BaseObject is IHasOffsetAngle offsetAngle)
                    offset += offsetAngle.GetOffsetAngle();

                Distance = Math.Abs(Extensions.GetDeltaAngle(firstAngled.Angle, (lastAngled.BaseObject.Angle + offset)));
                StrainTime = Math.Max(StrainTime, StartTime - lastAngled.StartTime);

                // Have to aim the entirety of the strict hard beat, so let's increase the distance manually
                if (lastAngled.BaseObject is StrictHardBeat strict)
                    Distance += (float)(strict.Range / 2);
            }

            if (hitObject is Slider slider)
            {
                TravelDistance = slider.Path.CalculatedDistance;
                LazyTravelDistance = slider.Path.CalculateLazyDistance((float)(AngleRange / 2));
                TravelTime = slider.Duration / clockRate;
            }
        }
    }
}
