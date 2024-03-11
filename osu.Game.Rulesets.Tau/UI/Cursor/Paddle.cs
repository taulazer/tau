using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public partial class Paddle : Container
    {
        public const float PADDLE_RADIUS = 0.05f;

        private readonly CircularProgress paddle;

        public Paddle()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Colour = TauPlayfield.ACCENT_COLOUR.Value;

            InternalChildren = new Drawable[]
            {
                paddle = new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Progress = 0,
                    InnerRadius = PADDLE_RADIUS
                },
                new HandlePiece()
            };
        }

        private readonly BindableDouble angleRange = new(75);

        [BackgroundDependencyLoader(true)]
        private void load(TauCachedProperties props)
        {
            if (props != null)
                angleRange.BindTo(props.AngleRange);

            angleRange.BindValueChanged(r =>
            {
                paddle.Progress = r.NewValue / 360;
                paddle.Rotation = (float)(-r.NewValue / 2);
            }, true);
        }
    }
}
