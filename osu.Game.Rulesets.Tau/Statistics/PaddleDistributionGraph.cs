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
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Screens.Ranking.Expanded.Accuracy;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Statistics
{
    public class PaddleDistributionGraph : CompositeDrawable
    {
        private Container beatsBarContainer;
        private Container slidersBarContainer;

        private readonly TauCachedProperties properties = new();
        private readonly IReadOnlyList<HitEvent> beatHitEvents;
        private readonly IReadOnlyList<HitEvent> sliderHitEvents;

        private readonly BindableBool showSliders = new(true);
        private readonly BindableBool showBeats = new(true);

        private double angleRange => properties.AngleRange.Value;

        public PaddleDistributionGraph(IReadOnlyList<HitEvent> hitEvents, IBeatmap beatmap)
        {
            beatHitEvents = hitEvents.Where(e => e.HitObject.HitWindows is not HitWindows.EmptyHitWindows && e.HitObject is Beat && e.Result.IsHit()).ToList();
            sliderHitEvents = hitEvents.Where(e => e.HitObject is Slider && e.Result.IsHit()).ToList(); // Note that this will only count the end of the sliders.
            properties.SetRange(beatmap.Difficulty.CircleSize);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (beatHitEvents == null || beatHitEvents.Count == 0)
                return;

            var paddedAngleRange = angleRange + 2; // 2° padding horizontally

            FillMode = FillMode.Fit;

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer()
                {
                    Width = 150,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(4),
                    Children = new Drawable[]
                    {
                        new DistributionCheckbox
                        {
                            LabelText = "Sliders",
                            Current = { BindTarget = showSliders }
                        },
                        new DistributionCheckbox(OverlayColourScheme.Green)
                        {
                            LabelText = "Beats",
                            Current = { BindTarget = showBeats },
                        }
                    }
                },
                beatsBarContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Y,
                    Y = 0.05f,
                    Scale = new Vector2(1),
                    FillAspectRatio = 1,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
                slidersBarContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Y,
                    Y = 0.05f,
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
                            Colour = ColourInfo.GradientVertical(Color4.White, Color4.White.Opacity(-0.1f)),
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
                        new SmoothCircularProgress
                        {
                            RelativeSizeAxes = Axes.Both,
                            RelativePositionAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Current = new BindableDouble(paddedAngleRange / 360),
                            InnerRadius = 0.05f,
                            Rotation = (float)(-paddedAngleRange / 2),
                            Y = 0.0035f
                        }
                    }
                }
            };

            createBars();

            showSliders.BindValueChanged(v => { slidersBarContainer.FadeTo(v.NewValue ? 1f : 0.25f, 500, Easing.OutQuint); });
            showBeats.BindValueChanged(v => { beatsBarContainer.FadeTo(v.NewValue ? 1f : 0.25f, 500, Easing.OutQuint); });
        }

        private void createBars()
        {
            float radius = Height * 2;
            int totalDistributionBins = (int)angleRange + 1;

            int[] beatBins = calculateBins(totalDistributionBins, beatHitEvents);
            int[] sliderBins = calculateBins(totalDistributionBins, sliderHitEvents);

            int maxBeatCount = beatBins.Max();
            int maxSliderCount = sliderBins.Max();

            if (maxSliderCount > 0)
                for (int i = 0; i < sliderBins.Length; i++)
                    slidersBarContainer.Add(new Bar
                    {
                        Origin = Anchor.TopLeft,
                        Colour = sliderBins.Length / 2 == i ? Color4.White : Color4Extensions.FromHex("#00AAFF"),
                        Height = Math.Max(0.075f, (float)sliderBins[i] / maxSliderCount) * 0.3f,
                        Position = Extensions.GetCircularPosition(radius - 17, i - (float)(angleRange / 2)) + new Vector2(0, radius),
                    });

            if (maxBeatCount > 0)
                for (int i = 0; i < beatBins.Length; i++)
                    beatsBarContainer.Add(new Bar
                    {
                        Colour = sliderBins.Length / 2 == i ? Color4.White : Color4Extensions.FromHex("#66FFCC"),
                        Height = Math.Max(0.075f, (float)beatBins[i] / maxBeatCount) * 0.3f,
                        Position = Extensions.GetCircularPosition(radius - 17, i - (float)(angleRange / 2)) + new Vector2(0, radius),
                    });
        }

        private int[] calculateBins(int totalBins, IReadOnlyList<HitEvent> hitEvents)
        {
            int[] bins = new int[totalBins];

            foreach (var hit in hitEvents)
            {
                var angle = hit.Position?.X ?? 0;
                angle += (float)angleRange / 2;
                var index = Math.Clamp((int)MathF.Round(angle), 0, (int)angleRange);

                bins[index]++;
            }

            return bins;
        }

        private class Bar : CompositeDrawable
        {
            public Bar()
            {
                Anchor = Anchor.TopCentre;
                Origin = Anchor.TopRight;

                RelativeSizeAxes = Axes.Y;
                Width = 2.5f;

                InternalChild = new Circle { RelativeSizeAxes = Axes.Both };
            }
        }

        private class DistributionCheckbox : SettingsCheckbox
        {
            [Cached]
            private OverlayColourProvider colourProvider;

            public DistributionCheckbox(OverlayColourScheme scheme = OverlayColourScheme.Blue)
            {
                colourProvider = new OverlayColourProvider(scheme);
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Current.BindValueChanged(v =>
                {
                    var spriteText = Control.ChildrenOfType<OsuSpriteText>().FirstOrDefault();
                    spriteText.Font = OsuFont.GetFont(weight: v.NewValue ? FontWeight.SemiBold : FontWeight.Regular);
                }, true);
            }
        }
    }
}
