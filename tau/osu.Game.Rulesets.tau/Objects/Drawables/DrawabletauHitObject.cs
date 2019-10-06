// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.tau.Objects.Drawables
{
    public class DrawabletauHitObject : DrawableHitObject<TauHitObject>
    {
        private Box box;

        public DrawabletauHitObject(TauHitObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(10);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddInternal(box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Alpha = 0.2f
            });

            hitObject.Angle = hitObject.PositionToEnd.GetDegreesFromPosition(box.AnchorPosition) * 4;
            Rotation = hitObject.Angle;

            var a = hitObject.Angle *= (float)(Math.PI / 180);
            Position = new Vector2(50 * (float)Math.Cos(a), 50 * (float)Math.Sin(a));
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (Time.Current >= HitObject.StartTime)
            {
                ApplyResult(r => r.Type = true
                    ? HitResult.Perfect
                    : HitResult.Miss);
            }
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Idle:
                    break;

                case ArmedState.Hit:
                    box.ScaleTo(2, time_fade_hit / 3, Easing.OutCubic)
                        .FadeColour(Color4.Yellow, time_fade_hit / 3, Easing.OutQuint)
                        .FadeOut(time_fade_hit)
                        .Expire();
                    break;

                case ArmedState.Miss:
                    box.ScaleTo(0.5f, time_fade_miss, Easing.InCubic)
                        .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .FadeOut(time_fade_miss)
                        .Expire();
                    break;
            }
        }
    }
}
