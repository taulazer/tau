using System;
using System.Linq;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider
    {
        private void updatePath()
        {
            path.ClearVertices();
            var nodes = HitObject.Path.Nodes;
            if (nodes.Count == 0)
                return;

            double time = Time.Current - HitObject.StartTime + HitObject.TimePreempt;
            double startTime = Math.Max(time - HitObject.TimePreempt - FadeTime, nodes[0].Time);
            double midTime = Math.Max(time - HitObject.TimePreempt, nodes[0].Time);
            double endTime = Math.Min(time, nodes[^1].Time);

            if (time < startTime)
                return;

            bool capAdded = false;
            var polarPath = HitObject.Path;

            float radius = TauPlayfield.BaseSize.X / 2;

            float distanceAt(double t) => inversed
                                              ? (float)(2 * radius - (time - t) / HitObject.TimePreempt * radius)
                                              : (float)((time - t) / HitObject.TimePreempt * radius);

            void addVertex(double t, double angle)
            {
                var p = Extensions.FromPolarCoordinates(distanceAt(t), (float)angle);
                int index = (int)(t / trackingCheckpointInterval);

                path.AddVertex(new Vector3(p.X, p.Y, trackingCheckpoints.ValueAtOrLastOr(index, true) ? 1 : 0));
            }

            foreach (var segment in polarPath.SegmentsBetween((float)startTime, (float)endTime))
            {
                foreach (var node in segment.Split(excludeFirst: capAdded))
                {
                    addVertex(node.Time, node.Angle);
                }

                capAdded = true;
            }

            var midNode = polarPath.NodeAt((float)midTime);
            var pos = Extensions.FromPolarCoordinates(distanceAt(midNode.Time), midNode.Angle);

            path.Position = pos;
            path.OriginPosition = path.PositionInBoundingBox(pos);
        }

        private bool checkIfTracking()
            => IsWithinPaddle() && TauActionInputManager.PressedActions.Any(x => Actions.Contains(x));

        protected override float GetCurrentOffset()
            => Vector2.Zero.GetDegreesFromPosition(path.Position);

        private TauInputManager tauActionInputManager;

        internal TauInputManager TauActionInputManager
            => tauActionInputManager ??= GetContainingInputManager() as TauInputManager;
    }
}
