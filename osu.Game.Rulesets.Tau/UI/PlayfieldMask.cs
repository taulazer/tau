using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics.OpenGL.Vertices;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI
{
    public enum MaskingMode
    {
        FadeOut,
        FadeIn,
    }

    public class PlayfieldMaskingContainer : CompositeDrawable
    {
        private readonly PlayfieldMaskDrawable cover;

        public PlayfieldMaskingContainer(Drawable content, MaskingMode mode)
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = new FixedSizeBufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1.5f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new[]
                {
                    new Container
                    {
                        Size = TauPlayfield.BASE_SIZE,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Child = content
                    },
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(2f),
                        RelativeSizeAxes = Axes.Both,
                        Blending = new BlendingParameters
                        {
                            // Don't change the destination colour.
                            RGBEquation = BlendingEquation.Add,
                            Source = BlendingType.Zero,
                            Destination = BlendingType.One,
                            // Subtract the cover's alpha from the destination (points with alpha 1 should make the destination completely transparent).
                            AlphaEquation = BlendingEquation.Add,
                            SourceAlpha = BlendingType.Zero,
                            DestinationAlpha = mode == MaskingMode.FadeOut ? BlendingType.OneMinusSrcAlpha : BlendingType.SrcAlpha
                        },
                        Child = cover = new PlayfieldMaskDrawable()
                    }
                }
            };
        }

        /// <summary>
        /// The relative area that should be completely covered if it is FadingIn, or the visible area if it is FadingOut.
        /// </summary>
        public float Coverage
        {
            set => cover.ApertureSize = new Vector2(0, TauPlayfield.BASE_SIZE.Y / 2 * value);
        }

        private class FixedSizeBufferedContainer : BufferedContainer
        {
            [Resolved]
            private TauPlayfield tauPlayfield { get; set; }

            protected override Quad ComputeScreenSpaceDrawQuad()
            {
                var SSDQDrawinfo = DrawInfo;

                // We apply a counter rotation so that the SSDQ retains the non-rotated Quad
                SSDQDrawinfo.ApplyTransform(AnchorPosition, Vector2.One, -tauPlayfield.Rotation, Vector2.Zero, OriginPosition);

                return Quad.FromRectangle(DrawRectangle) * SSDQDrawinfo.Matrix;
            }
        }

        public class PlayfieldMaskDrawable : Drawable
        {
            private IShader shader;

            protected override DrawNode CreateDrawNode() => new PlayfieldMaskDrawNode(this);

            [BackgroundDependencyLoader]
            private void load(ShaderManager shaderManager)
            {
                RelativeSizeAxes = Axes.Both;
                Anchor = Origin = Anchor.Centre;
                shader = shaderManager.Load("PositionAndColour", "PlayfieldMask");
            }

            private Vector2 apertureSize;

            public Vector2 ApertureSize
            {
                get => apertureSize;
                set
                {
                    if (apertureSize == value) return;

                    apertureSize = value;
                    Invalidate(Invalidation.DrawNode);
                }
            }

            public Vector2 AperturePosition => ToParentSpace(OriginPosition);

            private class PlayfieldMaskDrawNode : DrawNode
            {
                protected new PlayfieldMaskDrawable Source => (PlayfieldMaskDrawable)base.Source;

                private IShader shader;
                private Quad screenSpaceDrawQuad;

                private Vector2 aperturePosition;
                private Vector2 apertureSize;

                private readonly VertexBatch<PositionAndColourVertex> quadBatch = new QuadBatch<PositionAndColourVertex>(1, 1);
                private readonly Action<TexturedVertex2D> addAction;

                public PlayfieldMaskDrawNode(PlayfieldMaskDrawable source)
                    : base(source)
                {
                    addAction = v => quadBatch.Add(new PositionAndColourVertex
                    {
                        Position = v.Position,
                        Colour = v.Colour
                    });
                }

                public override void ApplyState()
                {
                    base.ApplyState();

                    shader = Source.shader;
                    screenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
                    aperturePosition = Vector2Extensions.Transform(Source.AperturePosition, DrawInfo.Matrix);
                    apertureSize = Source.ApertureSize * DrawInfo.Matrix.ExtractScale().Xy;
                }

                public override void Draw(Action<TexturedVertex2D> vertexAction)
                {
                    base.Draw(vertexAction);

                    shader.Bind();

                    shader.GetUniform<Vector2>("aperturePos").UpdateValue(ref aperturePosition);
                    shader.GetUniform<Vector2>("apertureSize").UpdateValue(ref apertureSize);

                    DrawQuad(Texture.WhitePixel, screenSpaceDrawQuad, DrawColourInfo.Colour, vertexAction: addAction);

                    shader.Unbind();
                }

                protected override void Dispose(bool isDisposing)
                {
                    base.Dispose(isDisposing);
                    quadBatch?.Dispose();
                }
            }
        }
    }
}
