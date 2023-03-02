using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.Timing;
using osu.Game.Configuration;
using osu.Game.Graphics.OpenGL.Vertices;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Mods
{
    public partial class TauModFlashlight : TauModFlashlight<TauHitObject>
    {
        public override double ScoreMultiplier => 1.16;

        public override float DefaultFlashlightSize => 0;

        [SettingSource(typeof(ModStrings), nameof(ModStrings.FlashlightSizeName), nameof(ModStrings.FlashlightSizeDescription))]
        public override BindableFloat SizeMultiplier { get; } = new BindableFloat
        {
            MinValue = 0.5f,
            MaxValue = 1.5f,
            Default = 1f,
            Value = 1f,
            Precision = 0.1f
        };

        [SettingSource(typeof(ModStrings), nameof(ModStrings.FlashlightComboName), nameof(ModStrings.FlashlightComboDescription))]
        public override BindableBool ComboBasedSize { get; } = new BindableBool
        {
            Default = true,
            Value = true
        };

        protected override Flashlight CreateFlashlight() => new TauFlashlight(this);

        private partial class TauFlashlight : Flashlight, IRequireHighFrequencyMousePosition
        {
            public TauFlashlight(TauModFlashlight modFlashlight)
                : base(modFlashlight)
            {
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                FlashlightRotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition) - 90;

                return base.OnMouseMove(e);
            }

            protected override void OnComboChange(ValueChangedEvent<int> e)
            {
                this.TransformTo(nameof(FlashlightSize), GetSizeFor(e.NewValue), FLASHLIGHT_FADE_DURATION);
            }

            protected override string FragmentShader => "TriangularFlashlight";
        }
    }

    public abstract partial class TauModFlashlight<T> : ModFlashlight, IApplicableToDrawableRuleset<T>, IApplicableToScoreProcessor
        where T : HitObject
    {
        public const double FLASHLIGHT_FADE_DURATION = 800;
        protected readonly BindableInt Combo = new BindableInt();

        public void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        {
            Combo.BindTo(scoreProcessor.Combo);

            // Default value of ScoreProcessor's Rank in Flashlight Mod should be SS+
            scoreProcessor.Rank.Value = ScoreRank.XH;
        }

        public ScoreRank AdjustRank(ScoreRank rank, double accuracy)
            => rank switch
            {
                ScoreRank.X => ScoreRank.XH,
                ScoreRank.S => ScoreRank.SH,
                _ => rank
            };

        public virtual void ApplyToDrawableRuleset(DrawableRuleset<T> drawableRuleset)
        {
            var flashlight = CreateFlashlight();

            flashlight.RelativeSizeAxes = Axes.Both;
            flashlight.Colour = Color4.Black;

            flashlight.Combo.BindTo(Combo);
            drawableRuleset.Overlays.Add(flashlight);

            flashlight.Breaks = drawableRuleset.Beatmap.Breaks;
        }

        protected abstract Flashlight CreateFlashlight();

        public abstract partial class Flashlight : Drawable
        {
            public readonly BindableInt Combo = new BindableInt();

            private IShader shader;

            protected override DrawNode CreateDrawNode() => new FlashlightDrawNode(this);

            public override bool RemoveCompletedTransforms => false;

            public List<BreakPeriod> Breaks;

            private readonly BindableDouble defaultFlashlightSize = new(32);
            private readonly float sizeMultiplier;
            private readonly bool comboBasedSize;

            protected Flashlight(ModFlashlight modFlashlight)
            {
                sizeMultiplier = modFlashlight.SizeMultiplier.Value;
                comboBasedSize = modFlashlight.ComboBasedSize.Value;
            }

            [BackgroundDependencyLoader(true)]
            private void load(ShaderManager shaderManager, TauCachedProperties props)
            {
                shader = shaderManager.Load("PositionAndColour", FragmentShader);
                defaultFlashlightSize.BindTo(props?.AngleRange);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                FlashlightSize = GetSizeFor(0);
                Combo.ValueChanged += OnComboChange;

                using (BeginAbsoluteSequence(0))
                {
                    foreach (var breakPeriod in Breaks)
                    {
                        if (!breakPeriod.HasEffect)
                            continue;

                        if (breakPeriod.Duration < FLASHLIGHT_FADE_DURATION * 2) continue;

                        this.Delay(breakPeriod.StartTime + FLASHLIGHT_FADE_DURATION).FadeOutFromOne(FLASHLIGHT_FADE_DURATION);
                        this.Delay(breakPeriod.EndTime - FLASHLIGHT_FADE_DURATION).FadeInFromZero(FLASHLIGHT_FADE_DURATION);
                    }
                }
            }

            protected abstract void OnComboChange(ValueChangedEvent<int> e);

            protected abstract string FragmentShader { get; }

            protected float GetSizeFor(int combo)
            {
                float size = (float)defaultFlashlightSize.Value * sizeMultiplier;

                if (comboBasedSize)
                {
                    if (combo > 200)
                        size *= 0.8f;
                    else if (combo > 100)
                        size *= 0.9f;
                }

                return size;
            }

            private float flashlightRotation;

            protected float FlashlightRotation
            {
                get => flashlightRotation;
                set
                {
                    if (flashlightRotation == value) return;

                    flashlightRotation = value;
                    Invalidate(Invalidation.DrawNode);
                }
            }

            private float flashlightSize;

            protected float FlashlightSize
            {
                get => flashlightSize;
                set
                {
                    if (flashlightSize == value) return;

                    flashlightSize = value;
                    Invalidate(Invalidation.DrawNode);
                }
            }

            private float flashlightDim;

            public float FlashlightDim
            {
                get => flashlightDim;
                set
                {
                    if (flashlightDim == value) return;

                    flashlightDim = value;
                    Invalidate(Invalidation.DrawNode);
                }
            }

            private class FlashlightDrawNode : DrawNode
            {
                protected new Flashlight Source => (Flashlight)base.Source;

                private IShader shader;
                private Quad screenSpaceDrawQuad;
                private Vector2 centerPos;
                private float range;
                private float rotation;
                private float flashlightDim;

                private IVertexBatch<PositionAndColourVertex> quadBatch;
                private Action<TexturedVertex2D> addAction;

                public FlashlightDrawNode(Flashlight source)
                    : base(source)
                {
                }

                public override void ApplyState()
                {
                    base.ApplyState();

                    shader = Source.shader;
                    screenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
                    centerPos = Source.ScreenSpaceDrawQuad.Centre;
                    range = Source.flashlightSize / 180f * MathF.PI;
                    rotation = Source.flashlightRotation / 180f * MathF.PI;
                    flashlightDim = Source.FlashlightDim;
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

                    shader.GetUniform<Vector2>("centerPos").UpdateValue(ref centerPos);
                    shader.GetUniform<float>("range").UpdateValue(ref range);
                    shader.GetUniform<float>("rotation").UpdateValue(ref rotation);
                    shader.GetUniform<float>("flashlightDim").UpdateValue(ref flashlightDim);

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
