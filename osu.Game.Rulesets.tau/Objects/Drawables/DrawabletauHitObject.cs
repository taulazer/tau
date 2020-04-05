// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauHitObject : DrawableHitObject<TauHitObject>
    {
        public DrawableTauHitObject(TauHitObject hitObject)
            : base(hitObject)
        {
            Alpha = 0;
        }
    }
}
