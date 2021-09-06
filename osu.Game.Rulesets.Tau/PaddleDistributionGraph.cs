using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Layout;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau
{
    public class PaddleDistributionGraph : CompositeDrawable
    {
        private readonly IReadOnlyList<HitEvent> hitEvents;

        private const float bin_per_angle = 1f;
        private float radius;
        private float angleRange;
        private Container barsContainer;
        private readonly LayoutValue layout = new LayoutValue(Invalidation.DrawSize);

        public PaddleDistributionGraph(IReadOnlyList<HitEvent> hitEvents, IBeatmap beatmap)
        {
            this.hitEvents = hitEvents.Where(e => !(e.HitObject.HitWindows is HitWindows.EmptyHitWindows) && e.HitObject is Beat && e.Result.IsHit()).ToList();

            angleRange = (float)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.CircleSize, 75, 25, 10);

            AddLayout(layout);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (hitEvents == null || hitEvents.Count == 0)
                return;

            var paddedAngleRange = angleRange + (1 * 2); // 2° padding horizontally

            FillMode = FillMode.Fit;

            InternalChildren = new Drawable[]
            {
                barsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Y,
                    Y = 0.06f,
                    Scale = new Vector2(1),
                    FillAspectRatio = 1,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
                new BufferedContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    FillAspectRatio = 1,
                    FillMode = FillMode.Fit,
                    Scale = new Vector2(4),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            Height = 0.25f,
                            Child = new CircularContainer
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Both,
                                Masking = true,
                                BorderThickness = 2,
                                BorderColour = Color4.White,
                                Height = 4,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Alpha = 0,
                                    AlwaysPresent = true,
                                }
                            },
                        },
                        new Box
                        {
                            Blending = new BlendingParameters
                            {
                                // Don't change the destination colour.
                                RGBEquation = BlendingEquation.Add,
                                Source = BlendingType.Zero,
                                Destination = BlendingType.One,
                                // Subtract the cover's alpha from the destination (points with alpha 1 should make the destination completely transparent).
                                AlphaEquation = BlendingEquation.Add,
                                SourceAlpha = BlendingType.Zero,
                                DestinationAlpha = BlendingType.SrcAlpha,
                            },
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Height = 0.26f,
                            Colour = ColourInfo.GradientVertical(Color4.White, Color4.White.Opacity(-0.2f)),
                        }
                    }
                },
                new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    FillAspectRatio = 1,
                    FillMode = FillMode.Fit,
                    Scale = new Vector2(4),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientVertical(Color4.DarkGray.Darken(0.5f).Opacity(0.3f), Color4.DarkGray.Darken(0.5f).Opacity(0f)),
                            Height = 0.25f,
                        },
                        new CircularProgress
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Current = new BindableDouble(paddedAngleRange / 360),
                            InnerRadius = 0.05f,
                            Rotation = -paddedAngleRange / 2,
                        }
                    }
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            if (hitEvents == null || hitEvents.Count == 0)
                return;

            if (!layout.IsValid)
            {
                barsContainer.Clear();
                radius = (Height * 4) / 2;
                var bars = calculateBars();

                foreach (var bar in bars)
                {
                    var pos = Extensions.GetCircularPosition(radius - 17, (bar.Index * bin_per_angle) - (angleRange / 2));
                    pos.Y += radius;

                    barsContainer.Add(bar.With(b =>
                    {
                        b.Position = pos;
                        b.Height *= 0.3f;
                    }));
                }

                layout.Validate();
            }
        }

        private Bar[] calculateBars()
        {
            int totalDistributionBins = (int)(angleRange / bin_per_angle) + 1;

            int[] bins = new int[totalDistributionBins];

            foreach (var hit in hitEvents)
            {
                var angle = hit.Position?.X ?? 0;
                angle += angleRange / 2;

                var index = MathF.Round((int)(angle / bin_per_angle), MidpointRounding.AwayFromZero);

                bins[(int)index]++;
            }

            int maxCount = bins.Max();
            var bars = new Bar[totalDistributionBins];

            for (int i = 0; i < bars.Length; i++)
                bars[i] = new Bar
                {
                    Height = Math.Max(0.075f, (float)bins[i] / maxCount),
                    Index = i
                };

            return bars;
        }

        private class Bar : CompositeDrawable
        {
            public float Index { get; set; }

            public Bar()
            {
                Anchor = Anchor.TopCentre;
                Origin = Anchor.TopCentre;

                RelativeSizeAxes = Axes.Y;
                Width = 5;

                InternalChild = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#66FFCC")
                };
            }
        }
    }
}
