using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public class KiaiEffectContainer : CompositeDrawable, INeedsNewResult, IFollowsSlider
    {
        private readonly ClassicKiaiEffect classicEffect;
        private readonly TurbulenceKiaiEffect turbulenceEffect;
        private readonly Bindable<KiaiType> kiaiType = new();

        public KiaiEffectContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                classicEffect = new ClassicKiaiEffect { Alpha = 0 },
                turbulenceEffect = new TurbulenceKiaiEffect()
            };

            kiaiType.BindValueChanged(t =>
            {
                switch (t.NewValue)
                {
                    case KiaiType.Turbulence:
                        classicEffect.FadeTo(0f, 250, Easing.OutQuint);
                        turbulenceEffect.FadeTo(1f, 250, Easing.OutQuint);
                        break;

                    case KiaiType.Classic:
                        classicEffect.FadeTo(1f, 250, Easing.OutQuint);
                        turbulenceEffect.FadeTo(0f, 250, Easing.OutQuint);
                        break;

                    case KiaiType.None:
                    default:
                        classicEffect.FadeTo(0f, 250, Easing.OutQuint);
                        turbulenceEffect.FadeTo(0f, 250, Easing.OutQuint);
                        break;
                }
            });
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.KiaiType, kiaiType);
            kiaiType.TriggerChange();
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            classicEffect.OnNewResult(judgedObject, result);
            turbulenceEffect.OnNewResult(judgedObject, result);
        }

        public void UpdateSliderPosition(float angle)
        {
            classicEffect.UpdateSliderPosition(angle);
            turbulenceEffect.UpdateSliderPosition(angle);
        }
    }
}
