// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModHardRock : ModHardRock, IApplicableToHitObject
    {
        public override double ScoreMultiplier => 1.06;
        public override bool Ranked => true;

        public void ApplyToHitObject(HitObject hitObject)
        {
            var tauObject = (TauHitObject)hitObject;
            while (tauObject.Angle < 0) tauObject.Angle += 360;
            tauObject.Angle %= 360;

            var newAngle = 0.0f;
            if (tauObject.Angle >= 0 && tauObject.Angle < 180)
                newAngle = 90 + (90 - tauObject.Angle);
            else if (tauObject.Angle >= 180)
                newAngle = 270 + (270 - tauObject.Angle);

            tauObject.Angle = newAngle;
        }
    }
}
