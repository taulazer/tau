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

            var time = Time.Current - HitObject.StartTime + HitObject.TimePreempt;
            var startTime = Math.Max(time - HitObject.TimePreempt - FadeTime, nodes[0].Time);
            var midTime = Math.Max(time - HitObject.TimePreempt, nodes[0].Time);
            var endTime = Math.Min(time, nodes[^1].Time);

            if (time < startTime)
                return;

            int nodeIndex = 0;
            bool capAdded = false;

            generatePathSegmnt(ref nodeIndex, ref capAdded, time, startTime, midTime);
            var pos = path.Vertices.Any() ? path.Vertices[^1].Xy : Vector2.Zero;
            generatePathSegmnt(ref nodeIndex, ref capAdded, time, midTime, endTime);

            path.Position = pos;
            path.OriginPosition = path.PositionInBoundingBox(pos);
        }

        private void generatePathSegmnt(ref int nodeIndex, ref bool capAdded, double time, double startTime, double endTime)
        {
            var nodes = HitObject.Path.Nodes;
            if (nodeIndex >= nodes.Count)
                return;

            while (nodeIndex + 1 < nodes.Count && nodes[nodeIndex + 1].Time <= startTime)
                nodeIndex++;

            const double delta_time = 20;
            const double max_angle_per_ms = 5;
            var radius = TauPlayfield.BaseSize.X / 2;

            float distanceAt(double t) => inversed
                                              ? (float)(2 * radius - (time - t) / HitObject.TimePreempt * radius)
                                              : (float)((time - t) / HitObject.TimePreempt * radius);

            void addVertex(double t, double angle)
            {
                var p = Extensions.GetCircularPosition(distanceAt(t), (float)angle);
                var index = (int)(t / trackingCheckpointInterval);
                path.AddVertex(new Vector3(p.X, p.Y, index >= 0 && index < trackingCheckpoints.Count ? (trackingCheckpoints[index] ? 1 : 0) : 1));
            }

            do
            {
                var prevNode = nodes[nodeIndex];
                var nextNode = nodeIndex + 1 < nodes.Count ? nodes[nodeIndex + 1] : prevNode;

                var from = Math.Max(startTime, prevNode.Time);
                var to = Math.Min(endTime, nextNode.Time);
                var duration = nextNode.Time - prevNode.Time;

                var deltaAngle = Extensions.GetDeltaAngle(nextNode.Angle, prevNode.Angle);
                var anglePerMs = duration != 0 ? deltaAngle / duration : 0;
                var timeStep = Math.Min(delta_time, Math.Abs(max_angle_per_ms / anglePerMs));

                if (!capAdded)
                    addVertex(from, prevNode.Angle + anglePerMs * (from - prevNode.Time));
                for (var t = from + timeStep; t < to; t += timeStep)
                    addVertex(t, prevNode.Angle + anglePerMs * (t - prevNode.Time));
                if (duration != 0)
                    addVertex(to, prevNode.Angle + anglePerMs * (to - prevNode.Time));
                else
                    addVertex(to, nextNode.Angle);

                capAdded = true;
                nodeIndex++;
            } while (nodeIndex < nodes.Count && nodes[nodeIndex].Time < endTime);
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
