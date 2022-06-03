using System;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Difficulty.Preprocessing;

public class TauAngledDifficultyHitObject : TauDifficultyHitObject
{
    /// <summary>
    /// Normalised distance between the start and end position of this <see cref="TauAngledDifficultyHitObject"/>.
    /// </summary>
    public double TravelDistance { get; private set; }

    /// <summary>
    /// Normalised distance between the start and end position of this <see cref="TauAngledDifficultyHitObject"/> in the shortest span possible to account for slider cheesing.
    /// </summary>
    public double LazyTravelDistance { get; private set; }

    /// <summary>
    /// The time taken to travel through <see cref="TravelDistance"/>, with a minimum value of 25ms for a non-zero distance.
    /// </summary>
    public double TravelTime { get; private set; }

    private readonly TauCachedProperties properties;

    public new AngledTauHitObject BaseObject => (AngledTauHitObject)base.BaseObject;

    public TauAngledDifficultyHitObject LastAngled;

    public double AngleRange => properties.AngleRange.Value;

    public double Distance;

    public TauAngledDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, TauCachedProperties properties,
                                        TauAngledDifficultyHitObject lastAngled)
        : base(hitObject, lastObject, clockRate)
    {
        this.properties = properties;
        LastAngled = lastAngled;

        if (hitObject is AngledTauHitObject firstAngled && lastAngled != null)
        {
            float offset = 0;

            if (lastAngled.BaseObject is IHasOffsetAngle offsetAngle)
                offset = offsetAngle.GetOffsetAngle();

            Distance = Math.Abs(Math.Max(0, Extensions.GetDeltaAngle(firstAngled.Angle, (lastAngled.BaseObject.Angle + offset)) - AngleRange / 2));
            StrainTime = Math.Max(StrainTime, StartTime - lastAngled.StartTime);
        }

        if (hitObject is Slider slider)
        {
            TravelDistance = slider.Path.CalculatedDistance;
            LazyTravelDistance = slider.Path.CalculateLazyDistance((float)(AngleRange / 2));
            TravelTime = slider.Duration / clockRate;
        }
    }
}
