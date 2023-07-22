using System;
using System.Linq;
using System.Runtime.InteropServices;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Tau.Allocation;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableSlider
    {
        public partial class SliderPath
        {
            public class SliderPathDrawNode : DrawNode
            {
                [StructLayout(LayoutKind.Sequential)]
                public record struct SliderTexturedVertex2D : IVertex
                {
                    [VertexMember(2, VertexAttribPointerType.Float)]
                    public Vector2 Position;

                    [VertexMember(4, VertexAttribPointerType.Float)]
                    public Color4 Colour;

                    [VertexMember(2, VertexAttribPointerType.Float)]
                    public Vector2 TexturePosition;

                    [VertexMember(4, VertexAttribPointerType.Float)]
                    public Vector4 TextureRect;

                    [VertexMember(2, VertexAttribPointerType.Float)]
                    public Vector2 BlendRange;

                    [VertexMember(1, VertexAttribPointerType.Float)]
                    public float Result;
                }

                private const int max_resolution = 24;

                protected new SliderPath Source => (SliderPath)base.Source;

                private RentedArray<(Line line, float result)> segments;
                private RentedArray<(Quad quad, float alpha, bool result)> ticks;
                private RentedArray<Quad> innerTicks;

                private Texture texture;
                private Vector2 drawSize;
                private float radius;
                private IShader shader;

                private IUniformBuffer<SliderUniform> shaderData;

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                private record struct SliderUniform
                {
                    public UniformVector2 CenterPos;

                    private UniformPadding8 _p1;

                    public UniformVector4 HitColor;
                    public UniformFloat Range;
                    public UniformFloat FadeRange;
                    public UniformBool Reverse;

                    private UniformPadding4 _p2;
                    private UniformPadding12 _p3;
                    private UniformPadding4 _p4;
                }

                // We multiply the size param by 3 such that the amount of vertices is a multiple of the amount of vertices
                // per primitive (triangles in this case). Otherwise overflowing the batch will result in wrong
                // grouping of vertices into primitives.
                private IVertexBatch<SliderTexturedVertex2D> halfCircleBatch;
                private IVertexBatch<SliderTexturedVertex2D> quadBatch;

                public SliderPathDrawNode(SliderPath source)
                    : base(source)
                {
                }

                private Vector4 hitColour;
                private Vector2 centerPos;
                private float range;
                private float fadeRange;
                private bool reverse;

                public override void ApplyState()
                {
                    base.ApplyState();

                    segments.Dispose();
                    segments = MemoryPool<(Line line, float result)>.Shared.Rent(Source.segments);
                    ticks.Dispose();
                    ticks = MemoryPool<(Quad, float, bool)>.Shared.Rent(Source.Ticks.Count());
                    innerTicks.Dispose();
                    innerTicks = MemoryPool<Quad>.Shared.Rent(ticks.Length);
                    int k = 0;

                    foreach (var i in Source.Ticks)
                    {
                        ticks[k] = (i.DrawableBox.ScreenSpaceDrawQuad, i.DrawableBox.Alpha, i.Result?.Type == Rulesets.Scoring.HitResult.Great);
                        innerTicks[k] = i.InnerDrawableBox.ScreenSpaceDrawQuad;

                        k++;
                    }

                    texture = Source.Texture;
                    drawSize = Source.DrawSize;
                    radius = Source.PathRadius;
                    shader = Source.hitFadeTextureShader;

                    var center = Source.PositionInBoundingBox(Vector2.Zero);
                    var edge = Source.PositionInBoundingBox(new Vector2(Source.PathDistance, 0));
                    var fade = Source.PositionInBoundingBox(new Vector2(Source.PathDistance + FADE_RANGE, 0));

                    centerPos = Source.ToScreenSpace(center);
                    range = (Source.ToScreenSpace(edge) - centerPos).Length;
                    fadeRange = (Source.ToScreenSpace(fade) - centerPos).Length - range;
                    var c = Source.FadeColour;
                    hitColour = new Vector4(c.R, c.G, c.B, c.A);
                    reverse = Source.Reverse;
                }

                private Vector2 pointOnCircle(float angle) => new(MathF.Sin(angle), -MathF.Cos(angle));

                private Vector2 relativePosition(Vector2 localPos) => Vector2.Divide(localPos, drawSize);

                private Color4 colourAt(Vector2 localPos)
                    => DrawColourInfo.Colour.HasSingleColour
                           ? ((SRGBColour)DrawColourInfo.Colour).Linear
                           : DrawColourInfo.Colour.Interpolate(relativePosition(localPos)).Linear;

                private void addLineCap(Vector2 origin, float result, float theta, float thetaDiff, RectangleF texRect)
                {
                    const float step = MathF.PI / max_resolution;

                    float dir = Math.Sign(thetaDiff);
                    thetaDiff = dir * thetaDiff;

                    if (thetaDiff > MathF.PI)
                    {
                        thetaDiff = 2 * MathF.PI - thetaDiff;
                        dir = -dir;
                    }

                    int amountPoints = (int)Math.Ceiling(thetaDiff / step);

                    if (dir < 0)
                        theta += MathF.PI;

                    Vector2 current = origin + pointOnCircle(theta) * radius;
                    Color4 currentColour = colourAt(current);
                    current = Vector2Extensions.Transform(current, DrawInfo.Matrix);

                    Vector2 screenOrigin = Vector2Extensions.Transform(origin, DrawInfo.Matrix);
                    Color4 originColour = colourAt(origin);

                    for (int i = 1; i <= amountPoints; i++)
                    {
                        // Center point
                        halfCircleBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = new Vector2(screenOrigin.X, screenOrigin.Y),
                            TexturePosition = new Vector2(texRect.Right, texRect.Centre.Y),
                            Colour = originColour,
                            Result = result
                        });

                        // First outer point
                        halfCircleBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = new Vector2(current.X, current.Y),
                            TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                            Colour = currentColour,
                            Result = result
                        });

                        float angularOffset = Math.Min(i * step, thetaDiff);
                        current = origin + pointOnCircle(theta + dir * angularOffset) * radius;
                        currentColour = colourAt(current);
                        current = Vector2Extensions.Transform(current, DrawInfo.Matrix);

                        // Second outer point
                        halfCircleBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = new Vector2(current.X, current.Y),
                            TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                            Colour = currentColour,
                            Result = result
                        });
                    }
                }

                private void addLineQuads(Line line, float startResult, float endResult, RectangleF texRect)
                {
                    Vector2 ortho = line.OrthogonalDirection;
                    Line lineLeft = new Line(line.StartPoint + ortho * radius, line.EndPoint + ortho * radius);
                    Line lineRight = new Line(line.StartPoint - ortho * radius, line.EndPoint - ortho * radius);

                    Line screenLineLeft = new Line(Vector2Extensions.Transform(lineLeft.StartPoint, DrawInfo.Matrix),
                        Vector2Extensions.Transform(lineLeft.EndPoint, DrawInfo.Matrix));
                    Line screenLineRight = new Line(Vector2Extensions.Transform(lineRight.StartPoint, DrawInfo.Matrix),
                        Vector2Extensions.Transform(lineRight.EndPoint, DrawInfo.Matrix));
                    Line screenLine = new Line(Vector2Extensions.Transform(line.StartPoint, DrawInfo.Matrix),
                        Vector2Extensions.Transform(line.EndPoint, DrawInfo.Matrix));

                    quadBatch.Add(new SliderTexturedVertex2D
                    {
                        Position = new Vector2(screenLineRight.EndPoint.X, screenLineRight.EndPoint.Y),
                        TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                        Colour = colourAt(lineRight.EndPoint),
                        Result = endResult
                    });
                    quadBatch.Add(new SliderTexturedVertex2D
                    {
                        Position = new Vector2(screenLineRight.StartPoint.X, screenLineRight.StartPoint.Y),
                        TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                        Colour = colourAt(lineRight.StartPoint),
                        Result = startResult
                    });

                    // Each "quad" of the slider is actually rendered as 2 quads, being split in half along the approximating line.
                    // On this line the depth is 1 instead of 0, which is done properly handle self-overlap using the depth buffer.
                    // Thus the middle vertices need to be added twice (once for each quad).
                    Vector2 firstMiddlePoint = new Vector2(screenLine.StartPoint.X, screenLine.StartPoint.Y);
                    Vector2 secondMiddlePoint = new Vector2(screenLine.EndPoint.X, screenLine.EndPoint.Y);
                    Color4 firstMiddleColour = colourAt(line.StartPoint);
                    Color4 secondMiddleColour = colourAt(line.EndPoint);

                    for (int i = 0; i < 2; ++i)
                    {
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = firstMiddlePoint,
                            TexturePosition = new Vector2(texRect.Right, texRect.Centre.Y),
                            Colour = firstMiddleColour,
                            Result = startResult
                        });
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = secondMiddlePoint,
                            TexturePosition = new Vector2(texRect.Right, texRect.Centre.Y),
                            Colour = secondMiddleColour,
                            Result = endResult
                        });
                    }

                    quadBatch.Add(new SliderTexturedVertex2D
                    {
                        Position = new Vector2(screenLineLeft.EndPoint.X, screenLineLeft.EndPoint.Y),
                        TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                        Colour = colourAt(lineLeft.EndPoint),
                        Result = endResult
                    });

                    quadBatch.Add(new SliderTexturedVertex2D
                    {
                        Position = new Vector2(screenLineLeft.StartPoint.X, screenLineLeft.StartPoint.Y),
                        TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                        Colour = colourAt(lineLeft.StartPoint),
                        Result = startResult
                    });
                }

                private void updateVertexBuffer()
                {
                    var (line, result) = segments[0];
                    float theta = line.Theta;

                    // Offset by 0.5 pixels inwards to ensure we never sample texels outside the bounds
                    RectangleF texRect = texture.GetTextureRect(new RectangleF(0.5f, 0.5f, texture.Width - 1, texture.Height - 1));
                    addLineCap(line.StartPoint, result, theta + MathF.PI, MathF.PI, texRect);

                    for (int i = 1; i < segments.Length; ++i)
                    {
                        var (nextLine, nextResult) = segments[i];
                        float nextTheta = nextLine.Theta;
                        addLineCap(line.EndPoint, nextResult, theta, nextTheta - theta, texRect);

                        line = nextLine;
                        theta = nextTheta;
                    }

                    addLineCap(line.EndPoint, segments[^1].result, theta, MathF.PI, texRect);

                    for (int i = 0; i < segments.Length; i++)
                    {
                        var (segment, segResult) = segments[i];
                        addLineQuads(segment, result, segResult, texRect);

                        result = segResult;
                    }

                    foreach (var i in ticks)
                    {
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = i.quad.BottomRight,
                            TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                            Colour = Color4.White.Opacity(i.alpha), //colourAt( lineRight.EndPoint ),
                            Result = i.result ? 1 : 0
                        });
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = i.quad.BottomLeft,
                            TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                            Colour = Color4.White.Opacity(i.alpha), //colourAt( lineRight.StartPoint ),
                            Result = i.result ? 1 : 0
                        });
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = i.quad.TopLeft,
                            TexturePosition = new Vector2(texRect.Right, texRect.Centre.Y),
                            Colour = Color4.White.Opacity(i.alpha), //firstMiddleColour,
                            Result = i.result ? 1 : 0
                        });
                        quadBatch.Add(new SliderTexturedVertex2D
                        {
                            Position = i.quad.TopRight,
                            TexturePosition = new Vector2(texRect.Right, texRect.Centre.Y),
                            Colour = Color4.White.Opacity(i.alpha), //secondMiddleColour,
                            Result = i.result ? 1 : 0
                        });
                    }
                }

                public override void Draw(IRenderer renderer)
                {
                    base.Draw(renderer);

                    shaderData ??= renderer.CreateUniformBuffer<SliderUniform>();

                    shaderData.Data = shaderData.Data with
                    {
                        CenterPos = centerPos,
                        Range = range,
                        FadeRange = fadeRange,
                        HitColor = hitColour,
                        Reverse = reverse
                    };

                    //maskData ??= renderer.CreateUniformBuffer<MaskUniform>();

                    halfCircleBatch ??= renderer.CreateLinearBatch<SliderTexturedVertex2D>(max_resolution * 100 * 3, 10, PrimitiveTopology.Triangles);
                    quadBatch ??= renderer.CreateQuadBatch<SliderTexturedVertex2D>(200, 10);

                    if (texture?.Available != true || segments.Length == 0)
                        return;

                    shader.Bind();
                    shader.BindUniformBlock("m_sliderParameters", shaderData);

                    renderer.PushStencilInfo(new StencilInfo(stencilTest: true, testValue: 255, testFunction: BufferTestFunction.Always, passed: StencilOperation.Replace));

                    foreach (var i in innerTicks)
                    {
                        renderer.DrawQuad(renderer.WhitePixel, i, Color4.Transparent);
                    }

                    renderer.PopStencilInfo();
                    renderer.PushStencilInfo(new StencilInfo(stencilTest: true, testValue: 255, testFunction: BufferTestFunction.NotEqual, passed: StencilOperation.Keep));

                    texture.Bind();
                    updateVertexBuffer();

                    // not needed right now, but it clears the stencil mask
                    renderer.PushStencilInfo(new StencilInfo(stencilTest: true, testValue: 0, testFunction: BufferTestFunction.Always, passed: StencilOperation.Replace));
                    foreach (var i in innerTicks)
                    {
                        renderer.DrawQuad(renderer.WhitePixel, i, Color4.Transparent);
                    }
                    renderer.PopStencilInfo();

                    renderer.PopStencilInfo();
                    shader.Unbind();
                }

                protected override void Dispose(bool isDisposing)
                {
                    base.Dispose(isDisposing);

                    shaderData?.Dispose();
                    halfCircleBatch?.Dispose();
                    quadBatch?.Dispose();
                    segments.Dispose();
                    ticks.Dispose();
                    innerTicks.Dispose();
                }
            }
        }
    }
}
