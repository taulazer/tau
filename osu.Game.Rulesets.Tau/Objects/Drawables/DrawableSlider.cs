using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
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
            Size = new Vector2(768);
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
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            PathRadius = 5,
                        }
                    }
                },
            });
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            path.ClearVertices();

            bool ShouldBeCulled(SliderNode node)
            {
                return Time.Current > HitObject.StartTime + node.Time;
            }

            var cullPivot = HitObject.Nodes.LastOrDefault(x => ShouldBeCulled(x));
            var cullAmount = cullPivot is null ? 0 : HitObject.Nodes.IndexOf(cullPivot);

            Console.WriteLine(cullAmount.ToString());

            foreach (var node in HitObject.Nodes.Reverse().SkipLast(cullAmount))
            {
                double intersectTime = HitObject.StartTime + node.Time;
                float targetAngle = node.Angle;

                // This is oob
                if (node == cullPivot)
                {
                    var nextNode = HitObject.Nodes.GetNext(node);
                    if (nextNode == null)
                        break;

                    float difference = (nextNode.Angle - node.Angle) % 360;
                    if (difference > 180) difference -= 360;
                    else if (difference < -180) difference += 360;
                    targetAngle = (float)Interpolation.Lerp(node.Angle, node.Angle + difference, (Time.Current - intersectTime) / (nextNode.Time - node.Time));
                }

                float distanceFromCentre = (float)Math.Clamp((Time.Current - (intersectTime - HitObject.TimePreempt)) / HitObject.TimePreempt, 0, 1) * 384;
                path.AddVertex(Extensions.GetCircularPosition(distanceFromCentre, targetAngle));
            }
            path.Position = path.Vertices.Any() ? path.Vertices.Last() : new Vector2(0);
            path.OriginPosition = path.Vertices.Any() ? path.PositionInBoundingBox(path.Vertices.Last()) : base.OriginPosition;
        }
    }
}
