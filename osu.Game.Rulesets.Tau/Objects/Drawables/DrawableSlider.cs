using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSlider : DrawableTauHitObject
    {
        private readonly Path path;

        public new Slider HitObject => base.HitObject as Slider;

        public DrawableSlider(TauHitObject obj)
            : base(obj)
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new CircularContainer
                {
                    Masking = true,
                    BorderThickness = 5,
                    BorderColour = Color4.White,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        },
                        path = new SmoothPath
                        {
                            RelativePositionAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Y = 0.5f,
                            PathRadius = 5
                        }
                    }
                },
            });
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            path.ClearVertices();
            path.AddVertex(Vector2.Zero);

            foreach (var node in HitObject.Nodes.Reverse())
            {
                var distanceFromCenter = (float)Math.Max(0, Time.Current - (HitObject.StartTime + node.Time) - HitObject.TimePreempt);
                path.AddVertex(Extensions.GetCircularPosition(distanceFromCenter, node.Angle));
            }

            path.OriginPosition = path.PositionInBoundingBox(path.Vertices[0]);
        }
    }
}
