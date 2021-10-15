using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
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
                new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.Handle), _ => new HandlePiece(), ConfineMode.ScaleToFit),
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
            var leftExplosion = new TriangleExplosion(RNG.Next(3, 5)) { Anchor = Anchor.Centre, Origin = Anchor.Centre };
            var rightExplosion = new TriangleExplosion(RNG.Next(3, 5)) { Anchor = Anchor.Centre, Origin = Anchor.Centre };

            ((TauCursor)Parent).Add(leftExplosion);
            ((TauCursor)Parent).Add(rightExplosion);

            paddle.TransformBindableTo(paddle.Current, angleRange / 360, 500, Easing.InExpo);
            paddle.RotateTo(-angleRange / 2, 500, Easing.InExpo);

            // We're using a scheduler here because we require Rotation to be up-to-date when we're setting the position.
            Scheduler.AddDelayed(() =>
            {
                leftExplosion.Rotation = -90 + -(angleRange / 2) + Rotation;
                rightExplosion.Rotation = 90 + angleRange / 2 + Rotation;

                leftExplosion.Position = Extensions.GetCircularPosition(DrawHeight / 2 * (1 - 0.025f), -(angleRange / 2) + Rotation);
                rightExplosion.Position = Extensions.GetCircularPosition(DrawHeight / 2 * (1 - 0.025f), (angleRange / 2) + Rotation);

                leftExplosion.Show();
                rightExplosion.Show();
            }, 500);
        }

        protected override void PopOut()
        {
            paddle.TransformBindableTo(paddle.Current, 0, 500, Easing.InExpo);
            paddle.RotateTo(0, 500, Easing.InExpo);
        }
    }
}
