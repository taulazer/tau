using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Rulesets.Tau.UI.Components;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        private readonly TauCursor cursor;
        private readonly Container judgementLayer;
        private readonly Container<KiaiHitExplosion> kiaiExplosionContainer;
        private readonly OrderedHitPolicy hitPolicy;
        private readonly IDictionary<HitResult, DrawablePool<DrawableTauJudgement>> poolDictionary = new Dictionary<HitResult, DrawablePool<DrawableTauJudgement>>();

        public static readonly Vector2 BASE_SIZE = new Vector2(768, 768);

        public static readonly Color4 ACCENT_COLOR = Color4Extensions.FromHex(@"FF0040");

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        private readonly CircularContainer ring;

        public TauPlayfield(BeatmapDifficulty difficulty)
        {
            RelativeSizeAxes = Axes.None;
            cursor = new TauCursor(difficulty);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(768);

            AddRangeInternal(new Drawable[]
            {
                judgementLayer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new VisualisationContainer(),
                new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Playfield), _ => new PlayfieldPiece()),
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
            });

            hitPolicy = new OrderedHitPolicy(HitObjectContainer);
            NewResult += onNewResult;

            var hitWindows = new TauHitWindows();

            foreach (var result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Where(r => r > HitResult.None && hitWindows.IsHitResultAllowed(r)))
                poolDictionary.Add(result, new DrawableJudgementPool(result, onJudgmentLoaded));

            AddRangeInternal(poolDictionary.Values);
        }

        private void onJudgmentLoaded(DrawableTauJudgement judgement)
        {
            judgementLayer.Add(judgement.GetProxyAboveHitObjectsContent());
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RegisterPool<Beat, DrawableBeat>(10);
            RegisterPool<HardBeat, DrawableHardBeat>(5);
            RegisterPool<Slider, DrawableSlider>(3);
        }

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableTauHitObject t)
            {
                t.CheckHittable = hitPolicy.IsHittable;
                t.CheckValidation = CheckIfWeCanValidate;
            }
        }

        protected override HitObjectLifetimeEntry CreateLifetimeEntry(HitObject hitObject) => new TauHitObjectLifetimeEntry(hitObject);

        public bool CheckIfWeCanValidate(float angle) => cursor.CheckForValidation(angle);

        [Resolved]
        private OsuColour colour { get; set; }

        public void CreateSliderEffect(float angle, bool kiai)
        {
            if ((int)Time.Current % (kiai ? 8 : 16) != 0) return;

            kiaiExplosionContainer.Add(new KiaiHitExplosion(ACCENT_COLOR, particleAmount: 1)
            {
                Position = Extensions.GetCircularPosition(.5f, angle),
                Angle = angle,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }

        private float cacheProgress;

        public void AdjustRingGlow(float progress, float angle)
        {
            if (cacheProgress == progress) return;
            cacheProgress = progress;

            var glow = cursor.PaddleDrawable.Glow;
            glow.FinishTransforms();

            glow.FadeTo(progress, progress == 0 ? 200 : 0);
            glow.Rotation = angle - cursor.PaddleDrawable.Rotation;

            glow.Line.Current.Value = Interpolation.ValueAt(progress, 0, 8f / 360, 0, 1, Easing.In);
            glow.Glow.Current.Value = Interpolation.ValueAt(progress, 0, 8f / 360, 0, 1, Easing.In);
            glow.Glow.Size = Interpolation.ValueAt(progress, new Vector2(0.6f), new Vector2(1.01f), 0, 1, Easing.In);
            glow.Glow.InnerRadius = Interpolation.ValueAt(progress, 0, 0.325f, 0, 1, Easing.In);
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            hitPolicy.HandleHit(judgedObject);

            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            judgementLayer.Add(poolDictionary[result.Type].Get(doj => doj.Apply(result, judgedObject)));

            if (judgedObject is DrawableSlider)
                cursor.PaddleDrawable.Glow.FadeOut(200);

            if (judgedObject.HitObject.Kiai && result.Type != HitResult.Miss)
            {
                float angle = judgedObject switch
                {
                    DrawableBeat b => b.HitObject.Angle,
                    DrawableSlider s => s.HitObject.Nodes.Last().Angle,
                    _ => 0
                };

                kiaiExplosionContainer.Add(new KiaiHitExplosion(colour.ForHitResult(judgedObject.Result.Type), judgedObject is DrawableHardBeat)
                {
                    Position = judgedObject is DrawableHardBeat ? Vector2.Zero : Extensions.GetCircularPosition(.5f, angle),
                    Angle = angle,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                });
            }
        }

        private class DrawableJudgementPool : DrawablePool<DrawableTauJudgement>
        {
            private readonly HitResult result;
            private readonly Action<DrawableTauJudgement> onLoaded;

            public DrawableJudgementPool(HitResult result, Action<DrawableTauJudgement> onLoaded)
                : base(10)
            {
                this.result = result;
                this.onLoaded = onLoaded;
            }

            protected override DrawableTauJudgement CreateNewDrawable()
            {
                var judgement = base.CreateNewDrawable();

                judgement.Apply(new JudgementResult(new HitObject(), new Judgement()) { Type = result }, null);

                onLoaded?.Invoke(judgement);

                return judgement;
            }
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
