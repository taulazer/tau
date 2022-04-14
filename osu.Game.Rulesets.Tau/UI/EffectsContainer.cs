using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.UI.Effects;

namespace osu.Game.Rulesets.Tau.UI
{
    public class EffectsContainer : CompositeDrawable
    {
        private readonly PlayfieldVisualizer visualizer;
        private readonly KiaiEffectContainer kiaiEffects;
        private readonly KiaiEffectContainer sliderEffects;

        private readonly Bindable<bool> showEffects = new(true);

        public EffectsContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                visualizer = new PlayfieldVisualizer(),
                kiaiEffects = new KiaiEffectContainer(),
                sliderEffects = new KiaiEffectContainer(),
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            visualizer.AccentColour = TauPlayfield.AccentColour.Value.Opacity(0.25f);

            config?.BindWith(TauRulesetSettings.ShowEffects, showEffects);
            showEffects.BindValueChanged(v => { this.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            showEffects.TriggerChange();
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!result.IsHit || !judgedObject.HitObject.Kiai)
                return;

            visualizer.OnNewResult(judgedObject, result);
            kiaiEffects.OnNewResult(judgedObject, result);
        }

        private const double tracking_threshold = 100;
        private double currentTrackingTime;

        public void TrackSlider(float angle)
        {
            currentTrackingTime += Time.Elapsed;

            if (currentTrackingTime < tracking_threshold)
                return;

            currentTrackingTime = 0;
            sliderEffects.UpdateSliderPosition(angle);
        }
    }
}
