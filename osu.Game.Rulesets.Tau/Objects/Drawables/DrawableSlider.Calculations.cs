using System;
using System.Linq;
using osu.Framework.Utils;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider
    {
        private void updatePath()
        {
            path.ClearVertices();

            float maxDistance = TauPlayfield.BaseSize.X / 2;

            for (double t = Math.Max(Time.Current, HitObject.StartTime + HitObject.Nodes.First().Time);
                 t < Math.Min(Time.Current + HitObject.TimePreempt, HitObject.StartTime + HitObject.Nodes.Last().Time);
                 t += 20)
            {
                var currentNode = HitObject.Nodes.Last(x => t >= HitObject.StartTime + x.Time);
                var currentNodeIndex = HitObject.Nodes.BinarySearch(currentNode);
                var nextNode = new Slider.SliderNode();

                if (currentNodeIndex < HitObject.Nodes.Count)
                    nextNode = HitObject.Nodes[currentNodeIndex + 1];

                double nodeStart = HitObject.StartTime + currentNode.Time;
                double nodeEnd = HitObject.StartTime + nextNode.Time;
                double duration = nodeEnd - nodeStart;

                float actualProgress = (float)((t - nodeStart) / duration);

                // Larger the time, the further in it is.
                float distanceFromCentre = (float)(1 - ((t - Time.Current) / HitObject.TimePreempt)) * maxDistance;

                if (inversed)
                    distanceFromCentre = (maxDistance * 2) - distanceFromCentre;

                // Angle calc
                float difference = (nextNode.Angle - currentNode.Angle) % 360;

                if (difference > 180) difference -= 360;
                else if (difference < -180) difference += 360;

                float targetAngle = (float)Interpolation.Lerp(currentNode.Angle, currentNode.Angle + difference, actualProgress);

                path.AddVertex(Extensions.GetCircularPosition(distanceFromCentre, targetAngle));
            }

            //Check if the last node is visible
            if (Time.Current + HitObject.TimePreempt > HitObject.StartTime + HitObject.Nodes.Last().Time)
            {
                double timeDiff = HitObject.StartTime + HitObject.Nodes.Last().Time - Time.Current;
                double progress = 1 - timeDiff / HitObject.TimePreempt;

                float endNodeDistanceFromCentre = (float)(progress * maxDistance);

                if (inversed)
                    endNodeDistanceFromCentre = (maxDistance * 2) - endNodeDistanceFromCentre;

                path.AddVertex(Extensions.GetCircularPosition(endNodeDistanceFromCentre, HitObject.Nodes.Last().Angle));
            }

            path.Position = path.Vertices.Any() ? path.Vertices.First() : Vector2.Zero;
            path.OriginPosition = path.Vertices.Any() ? path.PositionInBoundingBox(path.Vertices.First()) : base.OriginPosition;
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
