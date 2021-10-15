using Microsoft.EntityFrameworkCore;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class Paddle : VisibilityContainer
    {
        private readonly float angleRange;

        private readonly CircularProgress paddle;

        public Paddle(float angleRange)
        {
            this.angleRange = angleRange;

            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Colour = TauPlayfield.ACCENT_COLOR.Value;

            InternalChildren = new Drawable[]
            {
                paddle = new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(),
                    InnerRadius = 0.05f,
                },
                new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Handle), _ => new HandlePiece(), ConfineMode.ScaleToFit)
            };
        }

        [BackgroundDependencyLoader]
        private void load(ISkinSource skin)
        {
            Texture texture;

            if ((texture = skin.GetTexture("paddle")) != null)
                paddle.Texture = texture;
        }

        protected override void PopIn()
        {
            paddle.TransformBindableTo(paddle.Current, angleRange / 360, 500, Easing.OutExpo);
            paddle.RotateTo(-angleRange / 2, 500, Easing.OutExpo);
        }

        protected override void PopOut()
        {
            paddle.TransformBindableTo(paddle.Current, 0, 500, Easing.OutExpo);
            paddle.RotateTo(0, 500, Easing.OutExpo);
        }
    }
}
