// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;
using System.Linq;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableHardBeat : DrawableTauHitObject, IKeyBindingHandler<TauAction>
    {
        public override TauAction[] HitActions { get; set; } = new[]
        {
            TauAction.HardButton
        };

        private CircularContainer container;

        public DrawableHardBeat(TauHitObject hitObject)
            : base(hitObject)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.Zero;
            Alpha = 0f;
            AddRangeInternal(new Drawable[]
                {
                    container = new CircularContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(1),
                        Masking=true,
                        BorderThickness = 5,
                        BorderColour = Color4.White,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Alpha = 1f,
                        Children = new Drawable[]{
                            new Box{
                                RelativeSizeAxes= Axes.Both,
                                Alpha = 0,
                                AlwaysPresent = true
                            },
                        }
                    },
                }
            );

            Position = Vector2.Zero;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeIn(HitObject.TimeFadeIn);
            this.ResizeTo(1, HitObject.TimePreempt);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = result);
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    HitAction = null;

                    break;

                case ArmedState.Hit:
                    var b = HitObject.Angle;
                    var a = b *= (float)(Math.PI / 180);

                    this.ScaleTo(1.25f, time_fade_hit, Easing.OutQuint)
                        .FadeColour(Color4.Yellow, time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_hit);

                    container.TransformTo(nameof(container.BorderThickness), 0f, time_fade_hit, Easing.OutQuint);

                    break;

                case ArmedState.Miss:
                    var c = HitObject.Angle;
                    var d = c *= (float)(Math.PI / 180);

                    this.FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .ResizeTo(1.1f, time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_miss);

                    container.TransformTo(nameof(container.BorderThickness), 0f, time_fade_miss, Easing.OutQuint);

                    break;
            }
        }

        public bool OnPressed(TauAction action)
        {
            if (AllJudged)
                return false;

            if (HitActions.Contains(action))
                return UpdateResult(true);

            return false;
        }

        public void OnReleased(TauAction action)
        {
        }
    }
}
