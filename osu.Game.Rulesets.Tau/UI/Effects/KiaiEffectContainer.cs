using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public class KiaiEffectContainer : CompositeDrawable, INeedsNewResult
    {
        private readonly ClassicKiaiContainer classicContainer;
        private readonly TurbulenceKiaiContainer turbulenceContainer;
        private readonly Bindable<KiaiType> kiaiType = new();

        public KiaiEffectContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                classicContainer = new ClassicKiaiContainer { Alpha = 0 },
                turbulenceContainer = new TurbulenceKiaiContainer()
            };

            kiaiType.BindValueChanged(t =>
            {
                switch (t.NewValue)
                {
                    case KiaiType.Turbulence:
                        classicContainer.FadeTo(0f, 250, Easing.OutQuint);
                        turbulenceContainer.FadeTo(1f, 250, Easing.OutQuint);
                        break;

                    case KiaiType.Classic:
                        classicContainer.FadeTo(1f, 250, Easing.OutQuint);
                        turbulenceContainer.FadeTo(0f, 250, Easing.OutQuint);
                        break;

                    case KiaiType.None:
                    default:
                        classicContainer.FadeTo(0f, 250, Easing.OutQuint);
                        turbulenceContainer.FadeTo(0f, 250, Easing.OutQuint);
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
            classicContainer.OnNewResult(judgedObject, result);
            turbulenceContainer.OnNewResult(judgedObject, result);
        }
    }
}
