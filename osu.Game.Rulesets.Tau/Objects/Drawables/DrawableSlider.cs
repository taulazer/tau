using System;
using System.Diagnostics;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider : DrawableTauHitObject<Slider>
    {
        /// <summary>
        /// Check to see whether or not this Hit object is in the paddle's range.
        /// Also returns the amount of difference from the center of the paddle this Hit object was validated at.
        /// </summary>
        [CanBeNull]
        public Func<float, ValidationResult> CheckValidation;

        public SliderFollower Follower { get; private set; }

        private readonly Path path;
        private readonly Container<DrawableSliderHead> headContainer;
        private readonly Container<DrawableSliderRepeat> repeatContainer;
        private readonly Cached drawCache = new();

        public DrawableSlider()
            : this(null)
        {
        }

        public DrawableSlider(Slider obj)
            : base(obj)
        {
            Size = TauPlayfield.BaseSize;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new CircularContainer
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        },
                        path = new SmoothPath
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            PathRadius = 4,
                            AutoSizeAxes = Axes.None,
                            Size = TauPlayfield.BaseSize
                        },
                    }
                },
                Follower = new SliderFollower()
                {
                    AlwaysPresent = true,
                    Alpha = 0
                },
                headContainer = new Container<DrawableSliderHead> { RelativeSizeAxes = Axes.Both },
                repeatContainer = new Container<DrawableSliderRepeat> { RelativeSizeAxes = Axes.Both },
            });

            Follower.IsTracking.BindTo(Tracking);
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableSliderHead head:
                    headContainer.Child = head;
                    break;

                case DrawableSliderRepeat repeat:
                    repeatContainer.Add(repeat);
                    break;
            }
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
            => hitObject switch
            {
                SliderRepeat repeat => new DrawableSliderRepeat(repeat),
                SliderHeadBeat head => new DrawableSliderHead(head),
                _ => base.CreateNestedHitObject(hitObject)
            };

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();

            headContainer.Clear(false);
            repeatContainer.Clear(false);
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            host.DrawThread.Scheduler.AddDelayed(() => drawCache.Invalidate(), 0, true);
        }

        private double totalTimeHeld;

        protected override void OnApply()
        {
            base.OnApply();

            totalTimeHeld = 0;
        }

        public BindableBool Tracking = new();

        protected override void Update()
        {
            base.Update();

            // ReSharper disable once AssignmentInConditionalExpression
            if (Tracking.Value = checkIfTracking())
            {
                totalTimeHeld += Time.Elapsed;
            }

            // This gives us about the same performance as if we were to just would update the path without this.
            // The catch is that with this, we're giving the Update thread more breathing room to update everything
            // else instead of worrying with updating the path vertices every update frame.
            if (drawCache.IsValid)
                return;

            updatePath();
            Follower.UpdateProgress(getCurrentAngle());

            drawCache.Validate();
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeInFromZero(HitObject.TimeFadeIn);
        }

        protected override void UpdateStartTimeStateTransforms()
        {
            base.UpdateStartTimeStateTransforms();

            Follower.FadeIn();
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!(Time.Current > HitObject.GetEndTime())) return;

            double percentage = totalTimeHeld / HitObject.Duration;

            ApplyResult(r => r.Type = percentage switch
            {
                > .85 => HitResult.Great,
                > .50 => HitResult.Ok,
                _ => HitResult.Miss
            });
        }
    }
}
