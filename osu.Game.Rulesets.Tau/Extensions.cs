using System;
using System.Collections.Generic;
using osuTK;

namespace osu.Game.Rulesets.Tau
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the cartesian position from a polar coordinate.
        /// </summary>
        /// <param name="distance">The distance from the polar coordinate.</param>
        /// <param name="angle">The angle from the polar coordinate.</param>
        public static Vector2 FromPolarCoordinates(float distance, float angle)
            => new Vector2(
                -(distance * MathF.Cos((angle + 90f) * (MathF.PI / 180))),
                -(distance * MathF.Sin((angle + 90f) * (MathF.PI / 180)))
            );

        public static float Mod(float a, float b)
        {
            var m = a % b;
            return m < 0 ? (b + m) : m;
        }

        /// <summary>
        /// Gets the delta difference between two angles.
        /// </summary>
        public static float GetDeltaAngle(float a, float b)
            => Mod((a - b) + 180, 360) - 180;

        /// <summary>
        /// Gets the theta angle from two different points.
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        public static float GetDegreesFromPosition(this Vector2 a, Vector2 b)
        {
            Vector2 direction = b - a;
            float angle = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X));
            if (angle < 0f) angle += 360f;

            return angle + 90;
        }

        /// <summary>
        /// Normalizes the angle into a 0° -> 360° range.
        /// </summary>
        /// <param name="angle">The angle to normalize.</param>
        public static void NormalizeAngle(this ref float angle)
        {
            if (angle < 0) angle += 360;
            if (angle >= 360) angle %= 360;
        }

        /// <summary>
        /// Normalizes the angle into a 0° -> 360° range.
        /// </summary>
        /// <param name="angle">The angle to normalize.</param>
        public static float Normalize(this float angle)
        {
            if (angle < 0) angle += 360;
            if (angle >= 360) angle %= 360;

            return angle;
        }

        /// <summary>
        /// Remaps two set ranges of number into another.
        /// </summary>
        /// <param name="value">The current value from the first range.</param>
        /// <param name="x1">The first range's start value.</param>
        /// <param name="x2">The first range's end value.</param>
        /// <param name="y1">The second range's start value.</param>
        /// <param name="y2">The second range's end value.</param>
        public static float Remap(this float value, float x1, float x2, float y1, float y2)
        {
            var m = (y2 - y1) / (x2 - x1);
            var c = y1 - m * x1;

            return m * value + c;
        }

        public static T ValueAtOrLastOr<T>(this IList<T> self, int index, T @default = default)
            => index >= 0 && index < self.Count
                   ? self[index]
                   : self.Count > 0
                       ? self[^1]
                       : @default;
    }
}
