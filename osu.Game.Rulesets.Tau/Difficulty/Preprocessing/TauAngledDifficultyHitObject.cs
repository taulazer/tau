using System;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing
{
    public class TauAngledDifficultyHitObject : TauDifficultyHitObject
    {
        public new AngledTauHitObject BaseObject => (AngledTauHitObject)base.BaseObject;

        public new AngledTauHitObject LastObject => (AngledTauHitObject)base.LastObject;

        /// <summary>
        /// Normalised distance between the start and end position of this <see cref="OsuDifficultyHitObject"/>.
        /// </summary>
        public double TravelDistance { get; private set; }

        /// <summary>
        /// The time taken to travel through <see cref="TravelDistance"/>, with a minimum value of 25ms for a non-zero distance.
        /// </summary>
        public double TravelTime { get; private set; }

        public readonly double Distance;

        public TauAngledDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, TauCachedProperties properties)
            : base(hitObject, lastObject, clockRate, properties)
        {
            float distance = Math.Abs(Extensions.GetDeltaAngle(BaseObject.Angle, LastObject.Angle));
            Distance = distance - AngleRange;

            if (hitObject is Slider slider)
            {
                TravelDistance = slider.Path.CalculatedDistance;
                TravelTime = slider.Duration;
            }
        }
    }
}
