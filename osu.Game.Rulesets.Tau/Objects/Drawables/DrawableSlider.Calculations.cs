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
            var nodes = HitObject.Nodes;
            if (nodes.Count == 0)
                return;

            var radius = TauPlayfield.BaseSize.X / 2;
            var time = Time.Current - HitObject.StartTime + HitObject.TimePreempt;
            var startTime = Math.Max(time - HitObject.TimePreempt, nodes[0].Time);
            var endTime = Math.Min(time, nodes[^1].Time);

            const double delta_time = 20;
            const double max_angle_per_ms = 5;

            if (time < startTime)
                return;

            int nodeIndex = 0;
            while (nodeIndex + 1 < nodes.Count && nodes[nodeIndex + 1].Time <= startTime)
                nodeIndex++;

            float distanceAt(double t) => inversed
                                              ? (float)(2 * radius - (time - t) / HitObject.TimePreempt * radius)
                                              : (float)((time - t) / HitObject.TimePreempt * radius);

            bool capAdded = false;

            void addVertex(double t, double angle)
            {
                path.AddVertex(Extensions.GetCircularPosition(distanceAt(t), (float)angle));
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

            path.Position = path.Vertices[0];
            path.OriginPosition = path.PositionInBoundingBox(path.Vertices[0]);
        }

        private bool checkIfTracking()
            => IsWithinPaddle && TauActionInputManager.PressedActions.Any(x => Actions.Contains(x));

        private float getCurrentAngle()
            => Vector2.Zero.GetDegreesFromPosition(path.Position);

        public bool IsWithinPaddle
            => CheckValidation?.Invoke(getCurrentAngle()).IsValid ?? false;

        private TauInputManager tauActionInputManager;

        internal TauInputManager TauActionInputManager
            => tauActionInputManager ??= GetContainingInputManager() as TauInputManager;
    }
}
