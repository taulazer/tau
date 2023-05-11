using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider : DrawableAngledTauHitObject<Slider>
    {
        public Drawable SliderHead => headContainer.Child;

        private readonly BindableFloat size = new(16f);

        public float PathDistance = TauPlayfield.BASE_SIZE.X / 2;

        protected override TauAction[] Actions => HitObject.IsHard
            ? new[]
            {
                TauAction.HardButton1,
                TauAction.HardButton2
            }
            : base.Actions;

        private readonly SliderPath path;
        private readonly Container headContainer;
        private readonly Container<DrawableSliderTick> tickContainer;
        private readonly Container<DrawableSliderRepeat> repeatContainer;
        private readonly CircularContainer maskingContainer;
        private readonly Cached drawCache = new();
        private readonly PausableSkinnableSound slidingSample;

        private bool inversed;

        public DrawableSlider()
            : this(null)
        {
        }

        public DrawableSlider(Slider obj)
            : base(obj)
        {
            Size = TauPlayfield.BASE_SIZE;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                maskingContainer = new CircularContainer
                {
                    Masking = false,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        },
                        path = new SliderPath
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            PathRadius = 4,
                            PathDistance = PathDistance
                        },
                    }
                },
                headContainer = new Container { RelativeSizeAxes = Axes.Both },
                tickContainer = new Container<DrawableSliderTick> { RelativeSizeAxes = Axes.Both },
                repeatContainer = new Container<DrawableSliderRepeat> { RelativeSizeAxes = Axes.Both },
                slidingSample = new PausableSkinnableSound { Looping = true }
            });

            path.Ticks = repeatContainer;

            Tracking.BindValueChanged(updateSlidingSample);
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableSliderHead head:
                    headContainer.Child = head;
                    break;

                case DrawableSliderHardBeat head:
                    headContainer.Child = head;
                    break;


                case DrawableSliderRepeat repeat:
                    repeatContainer.Add(repeat);
                    break;

                case DrawableSliderTick tick:
                    tickContainer.Add(tick);
                    break;
            }
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
            => hitObject switch
            {
                SliderRepeat repeat => new DrawableSliderRepeat(repeat),
                SliderHeadBeat head => new DrawableSliderHead(head),
                SliderHardBeat head => new DrawableSliderHardBeat(head),
                SliderTick tick => new DrawableSliderTick(tick),
                _ => base.CreateNestedHitObject(hitObject)
            };

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();

            headContainer.Clear(false);
            repeatContainer.Clear(false);
            tickContainer.Clear(false);
        }

        private float convertNoteSizeToSliderSize(float beatSize)
            => Interpolation.ValueAt(beatSize, 2f, 7f, 10f, 25f);

        [BackgroundDependencyLoader]
        private void load(IRenderer renderer, GameHost host)
        {
            NoteSize.BindValueChanged(value => path.PathRadius = convertNoteSizeToSliderSize(value.NewValue), true);

            host.DrawThread.Scheduler.AddDelayed(() => drawCache.Invalidate(), 0, true);
            path.Texture = properties.SliderTexture ??= generateSmoothPathTexture(renderer, path.PathRadius, _ => Color4.White);
        }

        [Resolved]
        private TauCachedProperties properties { get; set; }

        private double totalTimeHeld;

        protected override void OnApply()
        {
            base.OnApply();
            path.FadeColour = colour.ForHitResult(HitResult.Great);

            totalTimeHeld = 0;

            if (properties.InverseModEnabled.Value)
            {
                inversed = true;
                maskingContainer.Masking = false;
                path.Reverse = inversed;
                // PathDistance = path.PathDistance = TauPlayfield.BaseSize.X;
            }
        }

        protected override void OnFree()
        {
            base.OnFree();
            trackingCheckpoints.Clear();
            slidingSample?.ClearSamples();
        }

        protected override void LoadSamples()
        {
            // Note: base.LoadSamples() isn't called since the slider plays the tail's hitsounds for the time being.

            Samples.Samples = HitObject.TailSamples.ToArray();
            slidingSample.Samples = HitObject.CreateSlidingSamples().ToArray();
        }

        public override void StopAllSamples()
        {
            base.StopAllSamples();
            slidingSample?.Stop();
        }

        private void updateSlidingSample(ValueChangedEvent<bool> tracking)
        {
            if (tracking.NewValue)
                slidingSample?.Play();
            else
                slidingSample?.Stop();
        }

        public BindableBool Tracking = new();
        private const double trackingCheckpointInterval = 5;
        private readonly List<bool> trackingCheckpoints = new();

        protected override void Update()
        {
            base.Update();

            // This gives us about the same performance as if we were to just would update the path without this.
            // The catch is that with this, we're giving the Update thread more breathing room to update everything
            // else instead of worrying with updating the path vertices every update frame.
            if (!drawCache.IsValid)
            {
                updatePath();
                drawCache.Validate();
            }

            if (Time.Current < HitObject.StartTime || Time.Current >= HitObject.GetEndTime()) return;

            // ReSharper disable once AssignmentInConditionalExpression
            if (Tracking.Value = checkIfTracking())
            {
                totalTimeHeld += Time.Elapsed;
            }

            var trackingCheckpointIndex = (int)((Time.Current - HitObject.StartTime) / trackingCheckpointInterval);

            if (trackingCheckpointIndex >= 0)
            {
                while (trackingCheckpoints.Count <= trackingCheckpointIndex)
                    trackingCheckpoints.Add(trackingCheckpoints.Count == 0 ? Tracking.Value : trackingCheckpoints[^1]);
                trackingCheckpoints[trackingCheckpointIndex] = Tracking.Value;
            }
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeInFromZero(HitObject.TimeFadeIn);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!(Time.Current > HitObject.GetEndTime())) return;

            double percentage = totalTimeHeld / HitObject.Duration;
            var result = percentage switch
            {
                > .85 => HitResult.Great,
                > .50 => HitResult.Ok,
                _ => HitResult.Miss
            };

            // Some nested hitobjects may not be judged before the tail, so we need to make sure that we have them all judged beforehand.
            // Thanks osu!.
            // ~ Nora
            foreach (var nested in NestedHitObjects.Where(n => !n.AllJudged))
            {
                if (nested is ICanApplyResult res)
                    res.ForcefullyApplyResult(r => r.Type = result);
            }

            ApplyResult(r =>
            {
                r.Type = result;
                ApplyCustomResult(r);
            });
        }

        protected override JudgementResult CreateResult(Judgement judgement)
            => new TauJudgementResult(HitObject, judgement);

        [Resolved]
        private OsuColour colour { get; set; }

        public double Velocity => PathDistance / HitObject.TimePreempt;
        public double FadeTime => FADE_RANGE / Velocity;

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            if (state is ArmedState.Hit or ArmedState.Miss)
                LifetimeEnd = Time.Current + FadeTime;
            else
                LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
        }
    }
}
