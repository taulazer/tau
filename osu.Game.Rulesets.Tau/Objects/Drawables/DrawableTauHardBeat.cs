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
using osu.Framework.Allocation;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.UI;
using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauHardBeat : DrawableHitObject<TauHitObject>, IKeyBindingHandler<TauAction>
    {
        /// <summary>
        /// A list of keys which can result in hits for this HitObject.
        /// </summary>
        public TauAction[] HitActions { get; set; } = new[]
        {
            TauAction.HardButton
        };

        /// <summary>
        /// The action that caused this <see cref="DrawableHit"/> to be hit.
        /// </summary>
        public TauAction? HitAction { get; private set; }

        private bool validActionPressed;

        protected sealed override double InitialLifetimeOffset => HitObject.TimePreempt;

        private CircularContainer ring;

        public DrawableTauHardBeat(TauHitObject hitObject)
            : base(hitObject)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;
            AddRangeInternal(new Drawable[]
                {
                    ring = new CircularContainer
                    {
                        Size = new Vector2(0),
                        Masking=true,
                        BorderThickness = 5,
                        BorderColour = Color4.White,
                        RelativePositionAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Alpha = 0.05f,
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

            ring.FadeIn(HitObject.TimeFadeIn);
            ring.ResizeTo(1, HitObject.TimePreempt)
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

                    ring.ScaleTo(2f, time_fade_hit, Easing.OutCubic)
                       .FadeColour(Color4.Yellow, time_fade_hit, Easing.OutCubic)
                       .FadeOut(time_fade_hit);

                    this.FadeOut(time_fade_hit);

                    break;

                case ArmedState.Miss:
                    var c = HitObject.Angle;
                    var d = c *= (float)(Math.PI / 180);

                    ring.FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                       .ResizeTo(1.1f, time_fade_hit, Easing.OutCubic)
                       .FadeOut(time_fade_miss);

                    this.FadeOut(time_fade_miss);

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
