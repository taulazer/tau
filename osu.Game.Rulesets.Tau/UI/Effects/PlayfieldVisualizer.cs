using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI.Effects
{
    public partial class PlayfieldVisualizer : Drawable
    {
        /// <summary>
        /// The maximum length of each bar in the visualiser.
        /// </summary>
        private const float bar_length = 600;

        /// <summary>
        /// The number of bars in one rotation of the visualiser.
        /// </summary>
        private const int bars_per_visualiser = 200;

        /// <summary>
        /// How many other bars should be affected when a Beat is hit.
        /// <remarks>The value here represents how much it should go in one side, how many bars will be affected will be the value * 2 + 1</remarks>
        /// </summary>
        private const int bar_spread = 15;

        /// <summary>
        /// How much should each bar go down each millisecond (based on a full bar).
        /// </summary>
        private const float decay_per_milisecond = 0.0024f;

        /// <summary>
        /// The minimum amplitude to show a bar.
        /// </summary>
        private const float amplitude_dead_zone = 1f / bar_length;

        /// <summary>
        /// How many times we should stretch around the circumference (overlapping overselves).
        /// </summary>
        private const int visualizer_rounds = 5;

        public Color4 AccentColour { get; set; }

        private readonly float[] amplitudes = new float[bars_per_visualiser];

        private IShader shader;
        private Texture texture;

        private readonly Bindable<bool> showVisualizer = new(true);

        public PlayfieldVisualizer()
        {
            Blending = BlendingParameters.Additive;
            RelativeSizeAxes = Axes.Both;
            FillMode = FillMode.Fit;
            FillAspectRatio = 1;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            AlwaysPresent = true; // This is to keep the update() function running, decaying the amplitudes.
            Alpha = 0;
        }

        private bool applyFade;

        public bool ApplyFade
        {
            get => applyFade;
            set
            {
                applyFade = value;
                Invalidate(Invalidation.DrawNode);
            }
        }

        [BackgroundDependencyLoader(true)]
        private void load(IRenderer renderer, ShaderManager shaders, TauRulesetConfigManager config)
        {
            texture = renderer.WhitePixel;
            shader = shaders.Load("VisualizerPositionAndColour", "VisualizerFade");

            config?.BindWith(TauRulesetSettings.ShowVisualizer, showVisualizer);
            showVisualizer.BindValueChanged(v => { this.FadeTo(v.NewValue ? 1 : 0, 250, Easing.OutQuint); }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            showVisualizer.TriggerChange();
        }

        public void OnNewResult(DrawableHitObject judgedObject)
        {
            switch (judgedObject.HitObject)
            {
                case IHasOffsetAngle offset:
                    UpdateAmplitudes(offset.GetAbsoluteAngle(), 0.5f);
                    break;

                case IHasAngle angle:
                    UpdateAmplitudes(angle.Angle, 0.5f);
                    break;

                case HardBeat _:
                    for (int i = 0; i < 360; i += 90)
                    {
                        UpdateAmplitudes(i, 0.5f);
                    }

                    break;
            }
        }

        /// <summary>
        /// Update the amplitudes for the visualizer.
        /// This spreads for the amount of <see cref="bar_spread"/>.
        /// </summary>
        /// <param name="angle">The angle that should be "hit".</param>
        /// <param name="multiplier">The multiplier for the amplitude.</param>
        public void UpdateAmplitudes(float angle, float multiplier)
        {
            var barIndex = Math.Clamp((int)angle.Remap(0, 360, 0, bars_per_visualiser), 0, bars_per_visualiser - 1);
            amplitudes[barIndex] += multiplier;

            for (int i = 1; i <= bar_spread; i++)
            {
                var indexLeft = barIndex - i;
                var indexRight = barIndex + i;

                if (indexLeft < 0)
                    indexLeft += bars_per_visualiser;

                if (indexRight >= bars_per_visualiser)
                    indexRight -= bars_per_visualiser;

                var amplitude = Interpolation.ValueAt(i, 0.0f, multiplier * 0.75f, bar_spread, 1, Easing.InQuint);
                amplitudes[indexLeft] += amplitude;
                amplitudes[indexRight] += amplitude;
            }
        }

        protected override void Update()
        {
            float decayFactor = (float)Math.Abs(Time.Elapsed) * decay_per_milisecond;

            for (int i = 0; i < bars_per_visualiser; i++)
            {
                //3% of extra bar length to make it a little faster when bar is almost at it's minimum
                amplitudes[i] -= decayFactor * (amplitudes[i] + 0.03f);

                if (amplitudes[i] < 0)
                    amplitudes[i] = 0;
            }

            Invalidate(Invalidation.DrawNode);
        }

        protected override DrawNode CreateDrawNode() => new PlayfieldVisualizerDrawNode(this);

        private class PlayfieldVisualizerDrawNode : DrawNode
        {
            protected new PlayfieldVisualizer Source => (PlayfieldVisualizer)base.Source;

            private IShader shader;
            private Texture texture;

            // Assuming the logo is a circle, we don't need a second dimension.
            private float size;
            private Vector2 center;
            private float radius;
            private float fadeRange;

            private Color4 colour;
            private float[] data;

            private IVertexBatch<TexturedVertex2D> vertexBatch;

            public PlayfieldVisualizerDrawNode(PlayfieldVisualizer source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                texture = Source.texture;
                size = Source.DrawSize.X;
                colour = Source.AccentColour;
                data = Source.amplitudes;
                center = Source.ScreenSpaceDrawQuad.Centre;
                radius = (Source.ToScreenSpace(Vector2.Zero) - Source.ToScreenSpace(new Vector2(TauPlayfield.BASE_SIZE.X / 2, 0))).Length;
                fadeRange = Source.ApplyFade ? radius * 0.2f : 0;
            }

            public override void Draw(IRenderer renderer)
            {
                base.Draw(renderer);

                vertexBatch ??= renderer.CreateQuadBatch<TexturedVertex2D>(100, 10);

                shader.Bind();
                shader.GetUniform<Vector2>("centerPos").UpdateValue(ref center);
                shader.GetUniform<float>("range").UpdateValue(ref radius);
                shader.GetUniform<float>("fadeRange").UpdateValue(ref fadeRange);

                Vector2 inflation = DrawInfo.MatrixInverse.ExtractScale().Xy;

                ColourInfo colourInfo = DrawColourInfo.Colour;
                colourInfo.ApplyChild(colour);

                if (data != null)
                {
                    for (int j = 0; j < visualizer_rounds; j++)
                    {
                        for (int i = 0; i < bars_per_visualiser; i++)
                        {
                            if (data[i] < amplitude_dead_zone)
                                continue;

                            float rotation = MathUtils.DegreesToRadians((i / (float)bars_per_visualiser * 360) + (j * 360 / (float)visualizer_rounds));
                            float rotationCos = MathF.Cos(rotation);
                            float rotationSin = MathF.Sin(rotation);
                            // taking the cos and sin to the 0..1 range
                            var barPosition = new Vector2((rotationCos / 2) + 0.5f, (rotationSin / 2) + 0.5f) * size;

                            var barSize = new Vector2(size * MathF.Sqrt(2 * (1 - MathF.Cos(MathUtils.DegreesToRadians(360f / bars_per_visualiser)))) / 2f,
                                bar_length * data[i]);
                            // The distance between the position and the sides of the bar.
                            var bottomOffset = new Vector2(-rotationSin * barSize.X / 2, rotationCos * barSize.X / 2);
                            // The distance between the bottom side of the bar and the top side.
                            var amplitudeOffset = new Vector2(rotationCos * barSize.Y, rotationSin * barSize.Y);

                            var rectangle = new Quad(
                                Vector2Extensions.Transform(barPosition - bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition - bottomOffset + amplitudeOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset, DrawInfo.Matrix),
                                Vector2Extensions.Transform(barPosition + bottomOffset + amplitudeOffset, DrawInfo.Matrix)
                            );

                            renderer.DrawQuad(texture,
                                rectangle,
                                colourInfo,
                                null,
                                vertexBatch.AddAction,
                                // barSize by itself will make it smooth more in the X axis than in the Y axis, this reverts that.
                                Vector2.Divide(inflation, barSize.Yx));
                        }
                    }
                }

                shader.Unbind();
            }
        }
    }
}
