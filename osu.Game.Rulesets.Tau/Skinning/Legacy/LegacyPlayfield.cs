using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.Skinning.Legacy
{
    public class LegacyPlayfield : CompositeDrawable
    {
        private Sprite background;

        [BackgroundDependencyLoader(true)]
        private void load(ISkinSource skin, TauRulesetConfigManager config)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            AddRangeInternal(new Drawable[]
            {
                background = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Texture = skin.GetTexture($"{TauRuleset.SHORT_NAME}-field-background")
                },
                new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Texture = skin.GetTexture($"{TauRuleset.SHORT_NAME}-ring-overlay")
                }
            });

            config?.BindWith(TauRulesetSettings.PlayfieldDim, playfieldDimLevel);
            playfieldDimLevel.BindValueChanged(v => { background.FadeTo(v.NewValue, 100); }, true);
        }

        private readonly Bindable<float> playfieldDimLevel = new Bindable<float>(0.3f);
    }
}
