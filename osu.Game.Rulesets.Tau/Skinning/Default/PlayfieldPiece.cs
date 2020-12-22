using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.UI;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Skinning.Default
{
    public class PlayfieldPiece : CompositeDrawable
    {
        private readonly Circle background;

        public PlayfieldPiece()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            AddRangeInternal(new Drawable[]
            {
                background = new Circle
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.3f
                },
                new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    BorderThickness = 3,
                    BorderColour = TauPlayfield.ACCENT_COLOR.Opacity(0.5f),
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        AlwaysPresent = true,
                        Alpha = 0,
                    }
                },
            });
        }

        private readonly Bindable<float> playfieldDimLevel = new Bindable<float>(0.3f);

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.PlayfieldDim, playfieldDimLevel);
            playfieldDimLevel.BindValueChanged(v => { background.FadeTo(v.NewValue, 100); }, true);
        }
    }
}
