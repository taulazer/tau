﻿using osu.Framework.Allocation;
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
        private readonly ClassicKiaiContainer classicKiaiContainer; // TODO: Replace this with a TauKiaiContainer which handles the logic of switching between different kiai effects.

        private readonly Bindable<bool> showEffects = new(true);
        private readonly Bindable<bool> showKiai = new(true);

        public EffectsContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                visualizer = new PlayfieldVisualizer(),
                classicKiaiContainer = new ClassicKiaiContainer()
            };

            showEffects.BindValueChanged(v => { this.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); });
            showKiai.BindValueChanged(v => { classicKiaiContainer.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); });
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            visualizer.AccentColour = TauPlayfield.AccentColour.Value.Opacity(0.25f);

            config?.BindWith(TauRulesetSettings.ShowEffects, showEffects);
            config?.BindWith(TauRulesetSettings.ShowKiai, showKiai);
            showEffects.TriggerChange();
            showKiai.TriggerChange();
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!result.IsHit || !judgedObject.HitObject.Kiai)
                return;

            foreach (var child in InternalChildren)
            {
                if (child is INeedsNewResult res)
                {
                    res.OnNewResult(judgedObject, result);
                }
            }
        }
    }
}