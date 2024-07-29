using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider
    {
        public const float FADE_RANGE = 120;

        private static Texture generateSmoothPathTexture(IRenderer renderer, float radius, Func<float, Color4> colourAt)
        {
            const float aa_portion = 0.02f;
            int textureWidth = (int)radius * 2;

            var raw = new SixLabors.ImageSharp.Image<Rgba32>(textureWidth, 1);

            for (int i = 0; i < textureWidth; i++)
            {
                float progress = (float)i / (textureWidth - 1);

                var colour = colourAt(progress);
                raw[i, 0] = new Rgba32(colour.R, colour.G, colour.B, colour.A * Math.Min(progress / aa_portion, 1));
            }

            var texture = renderer.CreateTexture(textureWidth, 1, true);
            texture.SetData(new TextureUpload(raw));
            return texture;
        }

        public partial class SliderPath : Drawable
        {
            public IEnumerable<DrawableSliderRepeat> Ticks = Array.Empty<DrawableSliderRepeat>();
            private IShader hitFadeTextureShader { get; set; }

            private readonly List<Vector3> vertices = new();

            public float PathDistance;

            public IReadOnlyList<Vector3> Vertices
            {
                get => vertices;
                set
                {
                    vertices.Clear();
                    vertices.AddRange(value);

                    vertexBoundsCache.Invalidate();
                    segmentsCache.Invalidate();

                    Invalidate(Invalidation.DrawSize);
                }
            }

            private float pathRadius = 10f;
            public Colour4 FadeColour;
            public bool Reverse;

            /// <summary>
            /// How wide this path is on each side of the line.
            /// </summary>
            /// <remarks>
            /// The actual width of the path is twice the PathRadius.
            /// </remarks>
            public virtual float PathRadius
            {
                get => pathRadius;
                set
                {
                    if (pathRadius == value) return;

                    pathRadius = value;

                    vertexBoundsCache.Invalidate();
                    segmentsCache.Invalidate();

                    Invalidate(Invalidation.DrawSize);
                }
            }

            public override Axes RelativeSizeAxes
            {
                get => base.RelativeSizeAxes;
                set
                {
                    if ((AutoSizeAxes & value) != 0)
                        throw new InvalidOperationException("No axis can be relatively sized and automatically sized at the same time.");

                    base.RelativeSizeAxes = value;
                }
            }

            private Axes autoSizeAxes;

            /// <summary>
            /// Controls which <see cref="Axes"/> are automatically sized w.r.t. the bounds of the vertices.
            /// It is not allowed to manually set <see cref="Size"/> (or <see cref="Width"/> / <see cref="Height"/>)
            /// on any <see cref="Axes"/> which are automatically sized.
            /// </summary>
            public virtual Axes AutoSizeAxes
            {
                get => autoSizeAxes;
                set
                {
                    if (value == autoSizeAxes)
                        return;

                    if ((RelativeSizeAxes & value) != 0)
                        throw new InvalidOperationException("No axis can be relatively sized and automatically sized at the same time.");

                    autoSizeAxes = value;
                    OnSizingChanged();
                }
            }

            public override float Width
            {
                get
                {
                    if (AutoSizeAxes.HasFlag(Axes.X))
                        return base.Width = vertexBounds.Width;

                    return base.Width;
                }
                set
                {
                    if ((AutoSizeAxes & Axes.X) != 0)
                        throw new InvalidOperationException($"The width of a {nameof(Path)} with {nameof(AutoSizeAxes)} can not be set manually.");

                    base.Width = value;
                }
            }

            public override float Height
            {
                get
                {
                    if (AutoSizeAxes.HasFlag(Axes.Y))
                        return base.Height = vertexBounds.Height;

                    return base.Height;
                }
                set
                {
                    if ((AutoSizeAxes & Axes.Y) != 0)
                        throw new InvalidOperationException($"The height of a {nameof(Path)} with {nameof(AutoSizeAxes)} can not be set manually.");

                    base.Height = value;
                }
            }

            public override Vector2 Size
            {
                get
                {
                    if (AutoSizeAxes != Axes.None)
                        return base.Size = vertexBounds.Size;

                    return base.Size;
                }
                set
                {
                    if ((AutoSizeAxes & Axes.Both) != 0)
                        throw new InvalidOperationException($"The Size of a {nameof(Path)} with {nameof(AutoSizeAxes)} can not be set manually.");

                    base.Size = value;
                }
            }

            private readonly Cached<RectangleF> vertexBoundsCache = new();

            private RectangleF vertexBounds
            {
                get
                {
                    if (vertexBoundsCache.IsValid)
                        return vertexBoundsCache.Value;

                    if (vertices.Count > 0)
                    {
                        float minX = 0;
                        float minY = 0;
                        float maxX = 0;
                        float maxY = 0;

                        foreach (var v in vertices)
                        {
                            minX = Math.Min(minX, v.X - PathRadius);
                            minY = Math.Min(minY, v.Y - PathRadius);
                            maxX = Math.Max(maxX, v.X + PathRadius);
                            maxY = Math.Max(maxY, v.Y + PathRadius);
                        }

                        return vertexBoundsCache.Value = new RectangleF(minX, minY, maxX - minX, maxY - minY);
                    }

                    return vertexBoundsCache.Value = new RectangleF(0, 0, 0, 0);
                }
            }

            public SliderPath()
            {
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(IRenderer renderer, ShaderManager shaders)
            {
                texture = renderer.WhitePixel;

                hitFadeTextureShader = shaders.Load("SliderPositionAndColour", "Slider");
            }

            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
            {
                var localPos = ToLocalSpace(screenSpacePos);
                float pathRadiusSquared = PathRadius * PathRadius;

                foreach (var (t, _) in segments)
                {
                    if (t.DistanceSquaredToPoint(localPos) <= pathRadiusSquared)
                        return true;
                }

                return false;
            }

            public Vector2 PositionInBoundingBox(Vector2 pos) => pos - vertexBounds.TopLeft;

            public void ClearVertices()
            {
                if (vertices.Count == 0)
                    return;

                vertices.Clear();

                vertexBoundsCache.Invalidate();
                segmentsCache.Invalidate();

                Invalidate(Invalidation.DrawSize);
            }

            /// <summary>
            /// Adds a vertex position to the slider.
            /// </summary>
            /// <param name="pos">The vertex position, where Z is used as the alpha.</param>
            public void AddVertex(Vector3 pos)
            {
                vertices.Add(pos);

                vertexBoundsCache.Invalidate();
                segmentsCache.Invalidate();

                Invalidate(Invalidation.DrawSize);
            }

            private readonly List<(Line, float)> segmentsBacking = new();
            private readonly Cached segmentsCache = new();
            private List<(Line, float)> segments => segmentsCache.IsValid ? segmentsBacking : generateSegments();

            private List<(Line, float)> generateSegments()
            {
                segmentsBacking.Clear();

                if (vertices.Count > 1)
                {
                    Vector2 offset = vertexBounds.TopLeft;
                    for (int i = 0; i < vertices.Count - 1; ++i)
                        segmentsBacking.Add((new Line(vertices[i].Xy - offset, vertices[i + 1].Xy - offset), vertices[i].Z));
                }

                segmentsCache.Validate();
                return segmentsBacking;
            }

            private Texture texture;

            public Texture Texture
            {
                get => texture;
                set
                {
                    if (texture == value)
                        return;

                    texture = value;
                    Invalidate(Invalidation.DrawNode);
                }
            }

            protected override DrawNode CreateDrawNode() => new SliderPathDrawNode(this);
        }
    }
}
