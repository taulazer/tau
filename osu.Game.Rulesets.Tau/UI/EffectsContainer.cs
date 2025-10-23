using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI.Effects;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Tau.UI
{
    public partial class EffectsContainer : CompositeDrawable
    {
        private readonly PlayfieldVisualizer visualizer;
        private readonly KiaiEffectContainer kiaiEffects;
        private readonly KiaiEffectContainer sliderEffects;

        private readonly Bindable<bool> showEffects = new(true);
        private readonly Bindable<bool> showSliderEffects = new(true);

        public EffectsContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren =
            [
                visualizer = new PlayfieldVisualizer(),
                kiaiEffects = new KiaiEffectContainer(),
                sliderEffects = new KiaiEffectContainer(40)
            ];
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config, IReadOnlyList<Mod> mods)
        {
            visualizer.AccentColour = TauPlayfield.ACCENT_COLOUR.Value.Opacity(0.25f);

            if (mods != null)
                visualizer.ApplyFade = mods.Any(x => x is TauModTraceable);

            config?.BindWith(TauRulesetSettings.ShowEffects, showEffects);
            config?.BindWith(TauRulesetSettings.ShowSliderEffects, showSliderEffects);
            showEffects.BindValueChanged(v => { this.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); }, true);
            showSliderEffects.BindValueChanged(v => { sliderEffects.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            showEffects.TriggerChange();
            showSliderEffects.TriggerChange();
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!result.IsHit || !judgedObject.HitObject.Kiai)
                return;

            visualizer.OnNewResult(judgedObject);
            kiaiEffects.OnNewResult(judgedObject, result);
        }

        private const double tracking_threshold = 75;
        private double currentTrackingTime;

        /// <summary>
        /// Updates any tracking children with the current angle.
        /// </summary>
        /// <param name="angle">The current angle the slider is at.</param>
        /// <param name="slider">The actual <see cref="DrawableSlider"/> that is emitting this function.</param>
        public void TrackSlider(float angle, DrawableSlider slider)
        {
            if (!slider.Tracking.Value || !slider.HitObject.Kiai)
                return;

            currentTrackingTime += Time.Elapsed;

            if (currentTrackingTime < tracking_threshold)
                return;

            currentTrackingTime = 0;
            visualizer.UpdateAmplitudes(angle, 0.15f);
        }
    }
}
