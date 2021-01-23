using System;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;

namespace osu.Game.Rulesets.Tau
{
    public static class Extensions
    {
        public static Vector2 GetCircularPosition(float distance, float angle)
            => new Vector2(-(distance * (float)Math.Cos((angle + 90f) * (float)(Math.PI / 180))), -(distance * (float)Math.Sin((angle + 90f) * (float)(Math.PI / 180))));

        public static Vector2 GetCircularPosition(this SliderNode node, float distance)
        => new Vector2(-(distance * (float)Math.Cos((node.Angle + 90f) * (float)(Math.PI / 180))), -(distance * (float)Math.Sin((node.Angle + 90f) * (float)(Math.PI / 180))));

        public static float GetDegreesFromPosition(this Vector2 a, Vector2 b)
        {
            Vector2 direction = b - a;
            float angle = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X));
            if (angle < 0f) angle += 360f;

            return angle + 90;
        }

        public static float GetDeltaAngle(float a, float b)
        {
            float x = b;
            float y = a;

            if (a > b)
            {
                x = a;
                y = b;
            }

            if (x - y < 180)
                x -= y;
            else
                x = 360 - x + y;

            return x;
        }

        public static float GetHitObjectAngle(this Vector2 target)
        {
            Vector2 offset = new Vector2(256, 192) - target; // Using centre of playfield.

            var tmp = (float)MathHelper.RadiansToDegrees(Math.Atan2(offset.X, -offset.Y)) - 180;

            return tmp.NormalizeAngle();
        }

        public static float NormalizeAngle(this float degrees)
        {
            if (degrees < 0) degrees += 360;
            if (degrees >= 360) degrees %= 360;

            return degrees;
        }
    }
}
