using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Timing;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Skinning;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSlider : DrawableTauHitObject, IKeyBindingHandler<TauAction>
    {
        private readonly Path path;

        public new Slider HitObject => base.HitObject as Slider;

        public DrawableSliderHead HeadBeat => headContainer.Child;

        private CircularContainer maskingContainer;

        private bool inversed;

        [Resolved(canBeNull: true)]
        private TauPlayfield playfield { get; set; }

        [Resolved]
        private OsuColour colour { get; set; }

        private Container<DrawableSliderHead> headContainer;
        private PausableSkinnableSound slidingSample;

        public DrawableSlider()
            : this(null)
        {
        }

        public DrawableSlider(TauHitObject obj)
            : base(obj)
        {
            Size = new Vector2(768);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                maskingContainer = new CircularContainer
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
                            PathRadius = 5,
                            AutoSizeAxes = Axes.None,
                            Size = TauPlayfield.BASE_SIZE
                        },
                        headContainer = new Container<DrawableSliderHead> { RelativeSizeAxes = Axes.Both },
                        slidingSample = new PausableSkinnableSound { Looping = true }
                    }
                },
            });
        }

        public void ApplyInverseChanges()
        {
            inversed = true;
            maskingContainer.Masking = false;

            // Max diameter of paths are much larger when they come from outside the ring, so we need extra canvas space
            path.Size = new Vector2(768 * 2);
        }

        private readonly Bindable<KiaiType> effect = new Bindable<KiaiType>();

        [BackgroundDependencyLoader(true)]
        private void load(ISkinSource skin, TauRulesetConfigManager config)
        {
            path.Colour = skin.GetConfig<TauSkinColour, Color4>(TauSkinColour.Slider)?.Value ?? Color4.White;
            config?.BindWith(TauRulesetSettings.KiaiEffect, effect);

            Tracking.BindValueChanged(updateSlidingSample);
        }

        protected override void OnApply()
        {
            base.OnApply();
            totalTimeHeld = 0;
        }

        protected override void OnFree()
        {
            base.OnFree();

            slidingSample.Samples = null;
        }

        protected override void LoadSamples()
        {
            base.LoadSamples();

            if (HitObject.SampleControlPoint == null)
            {
                throw new InvalidOperationException($"{nameof(HitObject)}s must always have an attached {nameof(HitObject.SampleControlPoint)}."
                                                    + $" This is an indication that {nameof(HitObject.ApplyDefaults)} has not been invoked on {this}.");
            }

            var slidingSamples = new List<ISampleInfo>();

            var normalSample = HitObject.Samples.FirstOrDefault(s => s.Name == HitSampleInfo.HIT_NORMAL);

            if (normalSample != null)
                slidingSamples.Add(HitObject.SampleControlPoint.ApplyTo(normalSample).With("sliderslide"));

            var whistleSample = HitObject.Samples.FirstOrDefault(s => s.Name == HitSampleInfo.HIT_WHISTLE);

            if (whistleSample != null)
                slidingSamples.Add(HitObject.SampleControlPoint.ApplyTo(whistleSample).With("sliderwhistle"));

            slidingSample.Samples = slidingSamples.ToArray();
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

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (Time.Current > HitObject.GetEndTime())
            {
                double percentage = totalTimeHeld / HitObject.Duration;

                HitResult result;

                if (percentage > .66) result = HitResult.Great;
                else if (percentage > .33) result = HitResult.Ok;
                else result = HitResult.Miss;

                ApplyResult(r => r.Type = result);
            }
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableSliderHead head:
                    headContainer.Child = head;

                    break;
            }
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case SliderHeadBeat head:
                    return new DrawableSliderHead(head);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();

            headContainer.Clear(false);
        }

        double totalTimeHeld = 0;

        public readonly Bindable<bool> Tracking = new Bindable<bool>();

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            path.ClearVertices();

            float maxDistance = TauPlayfield.BASE_SIZE.X / 2;

            for (double t = Math.Max(Time.Current, HitObject.StartTime + HitObject.Nodes.First().Time);
                 t < Math.Min(Time.Current + HitObject.TimePreempt, HitObject.StartTime + HitObject.Nodes.Last().Time);
                 t += 20) // Generate vertex every 20ms
            {
                var currentNode = HitObject.Nodes.Last(x => t >= HitObject.StartTime + x.Time);
                var nextNode = HitObject.Nodes.GetNext(currentNode);

                double nodeStart = HitObject.StartTime + currentNode.Time;
                double nodeEnd = HitObject.StartTime + nextNode.Time;
                double duration = nodeEnd - nodeStart;

                float actualProgress = (float)((t - nodeStart) / duration);

                // Larger the time, the further in it is.
                float distanceFromCentre = (float)(1 - ((t - Time.Current) / HitObject.TimePreempt)) * maxDistance;

                if (inversed)
                    distanceFromCentre = (maxDistance * 2) - distanceFromCentre;

                // Angle calc
                float difference = (nextNode.Angle - currentNode.Angle) % 360;

                if (difference > 180) difference -= 360;
                else if (difference < -180) difference += 360;

                float targetAngle = (float)Interpolation.Lerp(currentNode.Angle, currentNode.Angle + difference, actualProgress);

                path.AddVertex(Extensions.GetCircularPosition(distanceFromCentre, targetAngle));
            }

            //Check if the last node is visible
            if (Time.Current + HitObject.TimePreempt > HitObject.StartTime + HitObject.Nodes.Last().Time)
            {
                double timeDiff = HitObject.StartTime + HitObject.Nodes.Last().Time - Time.Current;
                double progress = 1 - (timeDiff / HitObject.TimePreempt);

                float endNodeDistanceFromCentre = (float)(progress * maxDistance);

                if (inversed)
                    endNodeDistanceFromCentre = (maxDistance * 2) - endNodeDistanceFromCentre;

                path.AddVertex(Extensions.GetCircularPosition(endNodeDistanceFromCentre, HitObject.Nodes.Last().Angle));
            }

            path.Position = path.Vertices.Any() ? path.Vertices.First() : new Vector2(0);
            path.OriginPosition = path.Vertices.Any() ? path.PositionInBoundingBox(path.Vertices.First()) : base.OriginPosition;

            if (IsWithinPaddle && TauActionInputManager.PressedActions.Any(x => HitActions.Contains(x)))
            {
                if (Tracking.Value == false)
                    Tracking.Value = true;
            }
            else
            {
                if (Tracking.Value)
                    Tracking.Value = false;
            }

            if (Time.Current < HitObject.StartTime || Time.Current >= HitObject.GetEndTime()) return;

            if (IsWithinPaddle && TauActionInputManager.PressedActions.Any(x => HitActions.Contains(x)))
            {
                playfield?.CreateSliderEffect(Vector2.Zero.GetDegreesFromPosition(path.Position), HitObject.Kiai);
                totalTimeHeld += Time.Elapsed;

                if (!HitObject.Kiai)
                    return;

                var angle = Vector2.Zero.GetDegreesFromPosition(path.Position);
                Drawable particle = Empty();
                const int duration = 1500;

                switch (effect.Value)
                {
                    case KiaiType.Turbulent:
                        {
                            playfield.SliderParticleEmitter.AddParticle(angle, inversed);

                            break;
                        }

                    case KiaiType.Classic:
                        if ((int)Time.Current % 8 != 0)
                            break;

                        particle = new Box
                        {
                            Position = Extensions.GetCircularPosition(380, angle),
                            Rotation = (float)RNG.NextDouble() * 360f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(RNG.Next(1, 10)),
                            Clock = new FramedClock(),
                            Blending = BlendingParameters.Additive,
                            Colour = TauPlayfield.ACCENT_COLOR.Value
                        };

                        particle.MoveTo(Extensions.GetCircularPosition(inversed ? -((RNG.NextSingle() * 50) + 390) : ((RNG.NextSingle() * 50) + 390), angle), duration, Easing.OutQuint)
                                .ResizeTo(new Vector2(RNG.NextSingle(0, 5)), duration, Easing.OutQuint).FadeOut(duration).Expire();

                        playfield.SliderParticleEmitter.Add(particle);

                        break;
                }
            }

            if (AllJudged) return;

            if (Tracking.Value)
                playfield?.AdjustRingGlow((float)(totalTimeHeld / HitObject.Duration), Vector2.Zero.GetDegreesFromPosition(path.Position));
            else
                playfield?.AdjustRingGlow(0, Vector2.Zero.GetDegreesFromPosition(path.Position));
        }

        public bool OnPressed(KeyBindingPressEvent<TauAction> e) => HitActions.Contains(e.Action) && !Tracking.Value;

        public void OnReleased(KeyBindingReleaseEvent<TauAction> e)
        {
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;

                    break;

                case ArmedState.Hit:
                case ArmedState.Miss:
                    Expire();

                    break;
            }
        }

        public bool IsWithinPaddle => CheckValidation?.Invoke(Vector2.Zero.GetDegreesFromPosition(path.Position)).Item1 ?? false;

        private TauInputManager tauActionInputManager;
        internal TauInputManager TauActionInputManager => tauActionInputManager ??= GetContainingInputManager() as TauInputManager;
    }
}
