using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class Paddle : VisibilityContainer
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
                    Current = new BindableDouble(),
                    InnerRadius = PADDLE_RADIUS
                },
                new HandlePiece()
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load(TauCachedProperties props)
        {
            if (props != null)
                angleRange.BindTo(props.AngleRange);
        }

        private readonly BindableDouble angleRange = new(75);

        protected override void PopIn()
        {
            paddle.TransformBindableTo(paddle.Current, angleRange.Value / 360, 500, Easing.InExpo);
            paddle.RotateTo((float)(-angleRange.Value / 2), 500, Easing.InExpo);
        }

        protected override void PopOut()
        {
            paddle.TransformBindableTo(paddle.Current, 0, 500, Easing.InExpo);
            paddle.RotateTo(0, 500, Easing.InExpo);
        }
    }
}
