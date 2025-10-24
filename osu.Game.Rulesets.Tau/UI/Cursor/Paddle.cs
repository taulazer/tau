using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public partial class Paddle : Container
    {
        public const float PADDLE_RADIUS = 0.05f;

        private readonly CircularProgress paddle;
        private readonly IBindable<double> angleRange;

        public Paddle(IBindable<double> angleRange)
        {
            this.angleRange = angleRange.GetBoundCopy();

            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Colour = TauPlayfield.ACCENT_COLOUR.Value;

            Scale = new Vector2(1f + (PADDLE_RADIUS / 2f));

            paddle = new CircularProgress
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Progress = 0,
                InnerRadius = PADDLE_RADIUS
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                paddle,
                CreateHandlePiece().With(static h => h.Scale = new Vector2(0.99f))
            ];

            angleRange.BindValueChanged(a =>
            {
                paddle.Progress = a.NewValue / 360;
                paddle.Rotation = (float)(-a.NewValue / 2);
            }, true);
        }

        protected virtual CompositeDrawable CreateHandlePiece() => new HandlePiece();

        public record struct AngleValidationResult(bool IsValid, float Delta);

        public AngleValidationResult ValidateAngle(float cursorRotation, float angle)
        {
            var totalRotation = ((cursorRotation - 90) + Rotation).Normalize();
            // todo : find out where the "- 90" is coming from
            var angleDiff = Extensions.GetDeltaAngle(totalRotation, angle - 90);
            var isValid = Math.Abs(angleDiff) <= angleRange.Value / 2f;

            return new AngleValidationResult(isValid, angleDiff);
        }
    }
}
