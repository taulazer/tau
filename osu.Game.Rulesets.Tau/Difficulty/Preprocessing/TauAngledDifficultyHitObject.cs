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

        public readonly double Distance;

        public TauAngledDifficultyHitObject(HitObject hitObject, HitObject lastObject, double clockRate, TauCachedProperties properties)
            : base(hitObject, lastObject, clockRate, properties)
        {
            var distance = Math.Abs(Extensions.GetDeltaAngle(BaseObject.Angle, LastObject.Angle));
            Distance = distance - AngleRange;
        }
    }
}
