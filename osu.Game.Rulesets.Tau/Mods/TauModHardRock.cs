using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModHardRock : ModHardRock, IApplicableToHitObject
    {
        public override double ScoreMultiplier => 1.06;

        public void ApplyToHitObject(HitObject hitObject)
        {
            // TODO: Change slider's angles too.
            if (hitObject is not IHasAngle angledHitObject)
                return;

            var newAngle = angledHitObject.Angle;
            newAngle -= 180;
            newAngle.NormalizeAngle();

            angledHitObject.Angle = newAngle;
        }
    }
}
