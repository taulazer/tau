using System;
using System.Diagnostics;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
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

        private readonly Path path;
        private readonly Container<DrawableSliderHead> headContainer;
        private readonly Cached drawCache = new();

        public DrawableSlider(Slider obj)
            : base(obj)
        {
            Size = TauPlayfield.BASE_SIZE;
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
                            Size = TauPlayfield.BASE_SIZE
                        },
                    }
                },
                headContainer = new Container<DrawableSliderHead> { RelativeSizeAxes = Axes.Both },
            });
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            headContainer.Child = hitObject switch
            {
                DrawableSliderHead head => head,
                _ => headContainer.Child
            };
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
            => hitObject switch
            {
                SliderHeadBeat head => new DrawableSliderHead(head),
                _ => base.CreateNestedHitObject(hitObject)
            };

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();

            headContainer.Clear(false);
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            host.DrawThread.Scheduler.AddDelayed(() => drawCache.Invalidate(), 0, true);
        }

        private double totalTimeHeld;

        protected override void Update()
        {
            base.Update();

            if (checkIfTracking())
            {
                totalTimeHeld += Time.Elapsed;
            }

            // This gives us about the same performance as if we were to just would update the path without this.
            // The catch is that with this, we're giving the Update thread more breathing room to update everything
            // else instead of worrying with updating the path vertices every update frame.
            if (drawCache.IsValid)
                return;

            updatePath();

            drawCache.Validate();
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
