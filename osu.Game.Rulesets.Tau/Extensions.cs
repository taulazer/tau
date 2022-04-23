using System;
using System.Collections.Generic;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Rulesets.Tau
{
    public static class Extensions
    {
        public static Vector2 GetCircularPosition(float distance, float angle)
            => new Vector2(
                -(distance * MathF.Cos((angle + 90f) * (MathF.PI / 180))),
                -(distance * MathF.Sin((angle + 90f) * (MathF.PI / 180)))
            );

        public static float Mod(float a, float b)
        {
            var m = a % b;
            return m < 0 ? (b + m) : m;
        }

        public static float GetDeltaAngle(float a, float b)
            => Mod((a - b) + 180, 360) - 180;

        public static float RandomBetween(float min, float max)
        {
            var diff = max - min;

            return (RNG.NextSingle() * diff) + min;
        }

        public static float GetDegreesFromPosition(this Vector2 a, Vector2 b)
        {
            Vector2 direction = b - a;
            float angle = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X));
            if (angle < 0f) angle += 360f;

            return angle + 90;
        }

        public static float GetHitObjectAngle(this Vector2 target)
        {
            var offset = new Vector2(256, 192) - target;
            var temp = MathHelper.RadiansToDegrees(MathF.Atan2(offset.X, -offset.Y)) - 180;
            NormalizeAngle(ref temp);

            return temp;
        }

        public static void NormalizeAngle(this ref float angle)
        {
            if (angle < 0) angle += 360;
            if (angle >= 360) angle %= 360;
        }

        public static float Normalize(this float angle)
        {
            if (angle < 0) angle += 360;
            if (angle >= 360) angle %= 360;

            return angle;
        }

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
