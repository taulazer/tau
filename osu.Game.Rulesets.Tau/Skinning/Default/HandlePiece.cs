using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Skinning.Default
{
    public class HandlePiece : CompositeDrawable
    {
        private readonly Box topLine;
        private readonly Box bottomLine;
        private readonly CircularContainer circle;

        public HandlePiece()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                bottomLine = new Box
                {
                    EdgeSmoothness = new Vector2(1f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(1.25f, 0.235f)
                },
                topLine = new Box
                {
                    EdgeSmoothness = new Vector2(1f),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(1.25f, 0.235f)
                },
                circle = new CircularContainer
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Both,
                    Y = -.25f,
                    Size = new Vector2(.03f),
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Masking = true,
                    BorderColour = Color4.White,
                    BorderThickness = 4,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        AlwaysPresent = true,
                        Alpha = 0,
                    }
                }
            };
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            circle.Y = -Math.Clamp(Vector2.Distance(AnchorPosition, e.MousePosition) / DrawHeight, .015f, .45f);
            bottomLine.Height = -circle.Y - .015f;
            topLine.Height = .5f + circle.Y - .015f;

            return base.OnMouseMove(e);
        }
    }
}
