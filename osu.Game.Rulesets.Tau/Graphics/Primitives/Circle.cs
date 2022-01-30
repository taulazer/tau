using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Rulesets.Tau.Graphics.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Circle : IConvexPolygon
    {
        private readonly List<Vector2> vertices = new();
        private readonly Quad quad;

        public Circle(int resolution, Quad quad)
        {
            this.quad = quad;
            generateVertices(resolution);
        }

        private void generateVertices(int resolution)
        {
            vertices.Clear();

            const float twice_pi = MathF.PI * 2;

            for (int i = 0; i < resolution; i++)
            {
                vertices.Add(new Vector2
                {
                    X = ((quad.Width / 2) * MathF.Cos(i * twice_pi / resolution)) + quad.TopLeft.X + (quad.Width / 2),
                    Y = ((quad.Height / 2) * MathF.Sin(i * twice_pi / resolution)) + quad.TopLeft.Y + (quad.Height / 2)
                });
            }
        }

        public ReadOnlySpan<Vector2> GetAxisVertices() => vertices.ToArray();

        public ReadOnlySpan<Vector2> GetVertices() => vertices.ToArray();

        // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
        public bool Contains(Vector2 pos)
        {
            var inside = false;

            for (int i = 0, j = vertices.Count - 1; i < vertices.Count; j = i++)
            {
                if ((vertices[i].Y > pos.Y) != (vertices[j].Y > pos.Y) &&
                    pos.X < (vertices[j].X - vertices[i].X) * (pos.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public RectangleF AABBFloat
        {
            get
            {
                var xMin = vertices.Min(v => v.X);
                var yMin = vertices.Min(v => v.Y);
                var xMax = vertices.Max(v => v.X);
                var yMax = vertices.Max(v => v.Y);

                return new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
            }
        }
    }
}
