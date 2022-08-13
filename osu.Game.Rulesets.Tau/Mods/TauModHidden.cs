using System;
using System.ComponentModel;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Game.Graphics.OpenGL.Vertices;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osuTK;
using Container = osu.Framework.Graphics.Containers.Container;

namespace osu.Game.Rulesets.Tau.Mods
{
    public abstract class TauModHidden : ModHidden, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => Mode.GetDescription();
        public override string Description => @"Play with no beats and fading sliders.";
        public override double ScoreMultiplier => 1.06;
        public override Type[] IncompatibleMods => base.IncompatibleMods.Concat(new[] { typeof(TauModInverse) }).ToArray();

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            var playfield = (TauPlayfield)drawableRuleset.Playfield;
            playfield.ShouldShowPositionalEffects.Value = false;

            var hitObjectContainer = playfield.HitObjectContainer;
            var hocParent = (Container)playfield.HitObjectContainer.Parent;

            hocParent.Remove(hitObjectContainer);
            hocParent.Add(new PlayfieldMaskingContainer(hitObjectContainer, Mode) { Coverage = InitialCoverage });
        }

        protected abstract MaskingMode Mode { get; }
        protected abstract float InitialCoverage { get; }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }
    }

    public enum MaskingMode
    {
        [Description("Fade Out")]
        FadeOut,

        [Description("Fade In")]
        FadeIn,
    }

    public class PlayfieldMaskingContainer : CompositeDrawable
    {
        private readonly PlayfieldMaskDrawable cover;

        public PlayfieldMaskingContainer(Drawable content, MaskingMode mode)
        {
            Mode = mode;

            RelativeSizeAxes = Axes.Both;

            InternalChild = new BufferedContainer(new[] { RenderBufferFormat.D16 })
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

        public MaskingMode Mode { get; }

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

                private IVertexBatch<PositionAndColourVertex> quadBatch;
                private Action<TexturedVertex2D> addAction;

                public PlayfieldMaskDrawNode(PlayfieldMaskDrawable source)
                    : base(source)
                {
                }

                public override void ApplyState()
                {
                    base.ApplyState();

                    shader = Source.shader;
                    screenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
                    aperturePosition = Vector2Extensions.Transform(Source.AperturePosition, DrawInfo.Matrix);
                    apertureSize = Source.ApertureSize * DrawInfo.Matrix.ExtractScale().Xy;
                }

                public override void Draw(IRenderer renderer)
                {
                    base.Draw(renderer);

                    if (quadBatch == null)
                    {
                        quadBatch ??= renderer.CreateQuadBatch<PositionAndColourVertex>(1, 1);
                        addAction = v => quadBatch.Add(new PositionAndColourVertex
                        {
                            Position = v.Position,
                            Colour = v.Colour
                        });
                    }

                    shader.Bind();

                    shader.GetUniform<Vector2>("aperturePos").UpdateValue(ref aperturePosition);
                    shader.GetUniform<Vector2>("apertureSize").UpdateValue(ref apertureSize);

                    renderer.DrawQuad(renderer.WhitePixel, screenSpaceDrawQuad, DrawColourInfo.Colour, vertexAction: addAction);

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
