using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.Configuration;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI {
    public class PlayfieldPiece : CompositeDrawable
    {
        private readonly Box background;
        private readonly Bindable<float> playfieldDimLevel = new(0.7f);

        public PlayfieldPiece()
        {
            RelativeSizeAxes = Axes.Both;

            AddInternal(new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                BorderThickness = 3,
                BorderColour = TauPlayfield.AccentColour.Value,
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = playfieldDimLevel.Default,
                    AlwaysPresent = true
                }
            });
        }

        [Resolved(canBeNull: true)]
        private TauRulesetConfigManager config { get; set; }

        protected override void LoadComplete()
        {
            config?.BindWith(TauRulesetSettings.PlayfieldDim, playfieldDimLevel);

            playfieldDimLevel.BindValueChanged(v =>
            {
                background.FadeTo(v.NewValue, 100);
            }, true);
        }
    }
}
