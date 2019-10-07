// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.MathUtils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.tau.Objects.Drawables
{
    public class DrawabletauHitObject : DrawableHitObject<TauHitObject>
    {
        private Box box;

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

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
                Alpha = 0.05f
            });

            hitObject.Angle = hitObject.PositionToEnd.GetDegreesFromPosition(box.AnchorPosition) * 4;
            box.Rotation = hitObject.Angle;

            Position = Vector2.Zero;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();
            var b = HitObject.PositionToEnd.GetDegreesFromPosition(box.AnchorPosition) * 4;
            var a = b *= (float)(Math.PI / 180);

            box.FadeIn(HitObject.TimeFadeIn);
            this.MoveTo(new Vector2(-(225 * (float)Math.Cos(a)), (225 * (float)Math.Sin(a))), HitObject.TimePreempt);
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
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    this.Delay(HitObject.TimePreempt).FadeOut(500);
                    Expire(true);
                    break;

                case ArmedState.Hit:
                    var b = HitObject.PositionToEnd.GetDegreesFromPosition(box.AnchorPosition) * 4;
                    var a = b *= (float)(Math.PI / 180);

                    box.ScaleTo(2f, time_fade_hit, Easing.OutCubic)
                        .FadeColour(Color4.Yellow, time_fade_hit, Easing.OutCubic)
                        .RotateTo(RNG.Next(-45, 45), time_fade_hit, Easing.OutCubic)
                        .MoveToOffset(new Vector2(20 * (float)Math.Cos(a), -(20 * (float)Math.Sin(a))), time_fade_hit, Easing.OutCubic)
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
