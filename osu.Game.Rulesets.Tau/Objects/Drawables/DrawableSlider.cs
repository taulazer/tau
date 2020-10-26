using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Scoring;
using osuTK;

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

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Great);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = result);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            path.ClearVertices();

            // Anything before Time.Current is NOT VISIBLE
            List<Vector2> vertices = new List<Vector2>();

            for (double t = Time.Current; t < Time.Current + HitObject.TimePreempt; t += 20) // Generate vertex every 20ms
            {
                var currentNode = HitObject.Nodes.LastOrDefault(x => t >= HitObject.StartTime + x.Time);
                if (currentNode == null) continue; // This is to ensure shit breaks... Because at least it is a controlled breakage ;)

                var nextNode = HitObject.Nodes.GetNext(currentNode);
                if (nextNode == null) break; // Can't break if you break it yourself. <Insert big brain meme here>


                double nodeStart = HitObject.StartTime + currentNode.Time;
                double nodeEnd = HitObject.StartTime + nextNode.Time;
                double duration = nodeEnd - nodeStart;

                float ActualProgress = (float)((t - nodeStart) / duration);

                // Larger the time, the further in it is.
                float distanceFromCentre = (float)(1 - ((t - Time.Current) / HitObject.TimePreempt)) * 384;

                // Angle calc
                float difference = (nextNode.Angle - currentNode.Angle) % 360;
                if (difference > 180) difference -= 360;
                else if (difference < -180) difference += 360;

                float targetAngle = (float)Interpolation.Lerp(currentNode.Angle, currentNode.Angle + difference, ActualProgress);

                vertices.Add(Extensions.GetCircularPosition(distanceFromCentre, targetAngle));
            }

            foreach (var v in vertices)
                path.AddVertex(v);

            path.Position = path.Vertices.Any() ? path.Vertices.Last() : new Vector2(0);
            path.OriginPosition = path.Vertices.Any() ? path.PositionInBoundingBox(path.Vertices.Last()) : base.OriginPosition;
        }
    }
}
