using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new(768);
        public static readonly Bindable<Color4> ACCENT_COLOUR = new(Color4Extensions.FromHex(@"FF0040"));

        protected override GameplayCursorContainer CreateCursor() => new TauCursor();
        public new TauCursor Cursor => base.Cursor as TauCursor;

        [Resolved]
        private TauCachedProperties tauCachedProperties { get; set; }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.None;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = BASE_SIZE;

            AddRangeInternal(new Drawable[]
            {
                new PlayfieldPiece(),
                HitObjectContainer
            });
        }

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableBeat b)
            {
                b.CheckValidation = checkPaddlePosition;
            }
        }

        private ValidationResult checkPaddlePosition(Beat tauHitObject)
        {
            var angleDiff = Extensions.GetDeltaAngle(Cursor.DrawablePaddle.Rotation, tauHitObject.Angle);

            return new ValidationResult(Math.Abs(angleDiff) <= tauCachedProperties.AngleRange.Value / 2, angleDiff);
        }

        private class PlayfieldPiece : CompositeDrawable
        {
            private readonly Bindable<float> playfieldDimLevel = new(0.8f);

            public PlayfieldPiece()
            {
                Box background;
                RelativeSizeAxes = Axes.Both;

                AddInternal(new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    BorderThickness = 3,
                    BorderColour = ACCENT_COLOUR.Value,
                    Child = background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = playfieldDimLevel.Value,
                        AlwaysPresent = true
                    }
                });

                playfieldDimLevel.BindValueChanged(v => { background.FadeTo(v.NewValue, 100); }, true);
            }
        }
    }
}
