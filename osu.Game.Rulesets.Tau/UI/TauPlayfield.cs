using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Timing;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI.Components;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.Tau.UI.Particles;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        private readonly Circle playfieldBackground;
        private readonly TauCursor cursor;
        private readonly JudgementContainer<DrawableTauJudgement> judgementLayer;
        private readonly Container<KiaiHitExplosion> kiaiExplosionContainer;
        private readonly OrderedHitPolicy hitPolicy;

        public readonly ParticleEmitter SliderParticleEmitter;

        public static readonly Vector2 BASE_SIZE = new Vector2(768, 768);
        public static readonly Color4 ACCENT_COLOR = Color4Extensions.FromHex(@"FF0040");

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public TauPlayfield(BeatmapDifficulty difficulty)
        {
            RelativeSizeAxes = Axes.None;
            cursor = new TauCursor(difficulty);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(768);

            AddRangeInternal(new Drawable[]
            {
                judgementLayer = new JudgementContainer<DrawableTauJudgement>
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new VisualisationContainer(),
                playfieldBackground = new Circle
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new CircularContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Masking = true,
                            BorderThickness = 3,
                            BorderColour = ACCENT_COLOR.Opacity(0.5f),
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                AlwaysPresent = true,
                                Alpha = 0,
                            }
                        },
                    }
                },
                HitObjectContainer,
                cursor,
                kiaiExplosionContainer = new Container<KiaiHitExplosion>
                {
                    Name = "Kiai hit explosions",
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Blending = BlendingParameters.Additive,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                },
                SliderParticleEmitter = new ParticleEmitter
                {
                    Name = "Slider particle emitter",
                    RelativeSizeAxes = Axes.Both,
                }
            });

            hitPolicy = new OrderedHitPolicy(HitObjectContainer);

            for (int i = 0; i < 8; i++)
            {
                SliderParticleEmitter.Vortices.Add(new Vortex
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Speed = RNG.NextSingle() * 10.5f + 2,
                    Scale = new Vector2(50),
                    Position = Extensions.GetCircularPosition(500f, (360 / 8) * i),
                    Velocity = Extensions.GetCircularPosition(50, (360 / 8) * i)
                });
            }
        }

        private readonly Bindable<KiaiType> effect = new Bindable<KiaiType>();
        protected Bindable<float> PlayfieldDimLevel = new Bindable<float>(0.3f); // Change the default as you see fit

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.PlayfieldDim, PlayfieldDimLevel);
            config?.BindWith(TauRulesetSettings.KiaiEffect, effect);
            PlayfieldDimLevel.ValueChanged += _ => updateVisuals();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            updateVisuals();
        }

        private void updateVisuals()
        {
            playfieldBackground.FadeTo(PlayfieldDimLevel.Value, 100);
        }

        public bool CheckIfWeCanValidate(float angle) => cursor.CheckForValidation(angle);

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            switch (h)
            {
                case DrawableTauHitObject obj:
                    obj.CheckValidation = CheckIfWeCanValidate;
                    obj.CheckHittable = hitPolicy.IsHittable;

                    break;
            }

            h.OnNewResult += onNewResult;
        }

        [Resolved]
        private OsuColour colour { get; set; }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            hitPolicy.HandleHit(judgedObject);

            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            DrawableTauJudgement explosion = new DrawableTauJudgement(result, judgedObject)
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
            };

            switch (judgedObject)
            {
                case DrawableSlider slider:
                    var sAngle = slider.HitObject.Nodes.Last().Angle;
                    explosion.Position = Extensions.GetCircularPosition(.6f, sAngle);
                    explosion.Rotation = sAngle;

                    if (judgedObject.HitObject.Kiai && result.Type != HitResult.Miss)
                    {
                        switch (effect.Value)
                        {
                            case KiaiType.Turbulent:
                                for (int i = 0; i < 20; i++)
                                {
                                    var particle = new Particle
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Position = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), sAngle),
                                        Velocity = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), randomBetween(sAngle - 40, sAngle + 40)),
                                        Size = new Vector2(RNG.NextSingle(1, 3)),
                                        Blending = BlendingParameters.Additive,
                                        Rotation = RNG.NextSingle(0, 360),
                                        Colour = colour.ForHitResult(judgedObject.Result.Type),
                                        Clock = new FramedClock()
                                    };

                                    particle.FadeOut(1500).Then().Expire();
                                    SliderParticleEmitter.Add(particle);
                                }

                                break;

                            case KiaiType.Classic:
                                kiaiExplosionContainer.Add(new KiaiHitExplosion(colour.ForHitResult(judgedObject.Result.Type))
                                {
                                    Position = Extensions.GetCircularPosition(.5f, sAngle),
                                    Angle = sAngle,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                });

                                break;
                        }
                    }

                    break;

                case DrawableBeat beat:
                    var angle = beat.HitObject.Angle;
                    explosion.Position = Extensions.GetCircularPosition(.6f, angle);
                    explosion.Rotation = angle;

                    if (judgedObject.HitObject.Kiai && result.Type != HitResult.Miss)
                    {
                        switch (effect.Value)
                        {
                            case KiaiType.Turbulent:
                                for (int i = 0; i < 20; i++)
                                {
                                    var particle = new Particle
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Position = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), angle),
                                        Velocity = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), randomBetween(angle - 40, angle + 40)),
                                        Size = new Vector2(RNG.NextSingle(1, 3)),
                                        Blending = BlendingParameters.Additive,
                                        Rotation = RNG.NextSingle(0, 360),
                                        Colour = colour.ForHitResult(judgedObject.Result.Type),
                                        Clock = new FramedClock()
                                    };

                                    particle.FadeOut(1500).Then().Expire();
                                    SliderParticleEmitter.Add(particle);
                                }

                                break;

                            case KiaiType.Classic:
                                kiaiExplosionContainer.Add(new KiaiHitExplosion(colour.ForHitResult(judgedObject.Result.Type))
                                {
                                    Position = Extensions.GetCircularPosition(.5f, angle),
                                    Angle = angle,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                });

                                break;
                        }
                    }

                    break;

                case DrawableHardBeat _:
                    explosion.Position = Extensions.GetCircularPosition(.6f, 0);

                    if (judgedObject.HitObject.Kiai && result.Type != HitResult.Miss)
                    {
                        switch (effect.Value)
                        {
                            case KiaiType.Turbulent:
                                for (int i = 0; i < 300; i++)
                                {
                                    var randomAngle = RNG.NextSingle(0, 360);

                                    var particle = new Particle
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Position = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), randomAngle),
                                        Velocity = Extensions.GetCircularPosition(RNG.NextSingle(380, 400), randomBetween(randomAngle - 40, randomAngle + 40)),
                                        Size = new Vector2(RNG.NextSingle(1, 3)),
                                        Blending = BlendingParameters.Additive,
                                        Rotation = RNG.NextSingle(0, 360),
                                        Colour = colour.ForHitResult(judgedObject.Result.Type),
                                        Clock = new FramedClock()
                                    };

                                    particle.FadeOut(1500).Then().Expire();
                                    SliderParticleEmitter.Add(particle);
                                }

                                break;

                            case KiaiType.Classic:
                                kiaiExplosionContainer.Add(new KiaiHitExplosion(colour.ForHitResult(judgedObject.Result.Type), true)
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                });

                                break;
                        }
                    }

                    break;
            }

            float randomBetween(float smallNumber, float bigNumber)
            {
                float diff = bigNumber - smallNumber;

                return ((float)RNG.NextDouble() * diff) + smallNumber;
            }

            judgementLayer.Add(explosion);
        }

        private class VisualisationContainer : BeatSyncedContainer
        {
            private PlayfieldVisualisation visualisation;
            private bool firstKiaiBeat = true;
            private int kiaiBeatIndex;
            private readonly Bindable<bool> showVisualisation = new Bindable<bool>(true);

            [BackgroundDependencyLoader(true)]
            private void load(TauRulesetConfigManager settings)
            {
                RelativeSizeAxes = Axes.Both;
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Child = visualisation = new PlayfieldVisualisation
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    FillAspectRatio = 1,
                    Blending = BlendingParameters.Additive,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Color4.Transparent
                };

                settings?.BindWith(TauRulesetSettings.ShowVisualizer, showVisualisation);
                showVisualisation.BindValueChanged(value => { visualisation.FadeTo(value.NewValue ? 1 : 0, 500); });
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                visualisation.AccentColour = ACCENT_COLOR.Opacity(0.5f);
                showVisualisation.TriggerChange();
            }

            protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
            {
                if (effectPoint.KiaiMode)
                {
                    kiaiBeatIndex += 1;

                    if (firstKiaiBeat)
                    {
                        visualisation.FlashColour(ACCENT_COLOR.Opacity(0.5f), timingPoint.BeatLength * 4, Easing.In);
                        firstKiaiBeat = false;

                        return;
                    }

                    if (kiaiBeatIndex >= 5)
                        visualisation.FlashColour(ACCENT_COLOR.Opacity(0.25f), timingPoint.BeatLength, Easing.In);
                }
                else
                {
                    firstKiaiBeat = true;
                    kiaiBeatIndex = 0;
                }
            }
        }
    }
}
