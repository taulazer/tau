using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public partial class TauPlayfield : Playfield
    {
        private readonly JudgementContainer<DrawableTauJudgement> judgementLayer;
        private readonly Container judgementAboveHitObjectLayer;

        internal readonly EffectsContainer EffectsContainer;

        public static readonly Vector2 BASE_SIZE = new(768);
        public static readonly Bindable<Color4> ACCENT_COLOUR = new(Color4Extensions.FromHex(@"FF0040"));

        private readonly Dictionary<HitResult, DrawablePool<DrawableTauJudgement>> poolDictionary = new();

        public BindableBool ShouldShowPositionalEffects = new(true);

        // don't like this.
        protected override GameplayCursorContainer CreateCursor()
        {
            if (Mods != null && Mods.Any(m => m is TauModRoundabout))
                return new TauModRoundabout.RoundaboutTauCursor();

            return new TauCursor();
        }

        public new TauCursor Cursor => (TauCursor)base.Cursor;

        [Resolved]
        private TauCachedProperties tauCachedProperties { get; set; }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public PlayfieldPiece PlayfieldPiece;

        public TauPlayfield()
        {
            RelativeSizeAxes = Axes.None;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = BASE_SIZE;

            AddRangeInternal([
                PlayfieldPiece = new PlayfieldPiece(),
                judgementLayer = new JudgementContainer<DrawableTauJudgement> { RelativeSizeAxes = Axes.Both },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = HitObjectContainer
                },
                EffectsContainer = new EffectsContainer(),
                judgementAboveHitObjectLayer = new Container { RelativeSizeAxes = Axes.Both }
            ]);

            NewResult += onNewResult;

            var hitWindows = new TauHitWindow();

            foreach (var result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Where(r => r > HitResult.None && hitWindows.IsHitResultAllowed(r)))
                poolDictionary.Add(result, new DrawableJudgementPool(result, onJudgmentLoaded));

            AddRangeInternal(poolDictionary.Values);
        }

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] IBeatmap beatmap)
        {
            RegisterPool<Beat, DrawableBeat>(10);
            RegisterPool<HardBeat, DrawableHardBeat>(5);
            RegisterPool<StrictHardBeat, DrawableStrictHardBeat>(5);

            RegisterPool<Slider, DrawableSlider>(5);
            RegisterPool<SliderHeadBeat, DrawableSliderHead>(5);
            RegisterPool<SliderHardBeat, DrawableSliderHardBeat>(5);
            RegisterPool<SliderRepeat, DrawableSliderRepeat>(5);
            RegisterPool<SliderTick, DrawableSliderTick>(10);

            if (beatmap != null)
                Cursor.SetAngleRange(beatmap.Difficulty.CircleSize);
        }

        private void onJudgmentLoaded(DrawableTauJudgement judgement)
        {
            judgementAboveHitObjectLayer.Add(judgement.ProxiedAboveHitObjectsContent);
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (ShouldShowPositionalEffects.Value)
                EffectsContainer.OnNewResult(judgedObject, result);

            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            judgementLayer.Add(poolDictionary[result.Type].Get(doj => doj.Apply(result, judgedObject)));
        }

        private partial class DrawableJudgementPool : DrawablePool<DrawableTauJudgement>
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
    }
}
