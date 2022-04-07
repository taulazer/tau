using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI.Effects;

namespace osu.Game.Rulesets.Tau.UI
{
    public class EffectsContainer : Container
    {
        private readonly PlayfieldVisualizer visualizer;

        private readonly Bindable<bool> showVisualizer = new();

        public EffectsContainer()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            Children = new Drawable[]
            {
                visualizer = new PlayfieldVisualizer { Alpha = 0 }
            };

            showVisualizer.BindValueChanged(v => { visualizer.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); });
        }

        [BackgroundDependencyLoader]
        private void load(TauRulesetConfigManager config)
        {
            visualizer.AccentColour = TauPlayfield.AccentColour.Value.Opacity(0.25f);

            config.BindWith(TauRulesetSettings.ShowVisualizer, showVisualizer);
        }

        public void OnNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!result.IsHit || !judgedObject.HitObject.Kiai)
                return;

            switch (judgedObject.HitObject)
            {
                case IHasAngle angle:
                    visualizer.UpdateAmplitudes(angle.Angle);
                    break;

                case HardBeat _:
                    for (int i = 0; i < 360; i += 90)
                    {
                        visualizer.UpdateAmplitudes(i);
                    }

                    break;
            }
        }
    }
}
