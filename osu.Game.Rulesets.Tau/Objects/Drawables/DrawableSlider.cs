using osu.Framework.Graphics;
using osu.Framework.Graphics.Lines;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableSlider : DrawableTauHitObject
    {
        private SmoothPath path;

        public new Slider HitObject => base.HitObject as Slider;

        public DrawableSlider(TauHitObject obj)
            : base(obj)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddInternal(path = new SmoothPath
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 0.5f,
                PathRadius = 5
            });
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            path.ClearVertices();
            path.AddVertex(Vector2.Zero);

            foreach (var node in HitObject.Nodes)
            {
                path.AddVertex(Extensions.GetCircularPosition(node.Time * 0.1f, node.Angle));
            }

            path.OriginPosition = path.PositionInBoundingBox(path.Vertices[0]);
        }
    }
}
