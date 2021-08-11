using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
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

        private const float scale = 2f;
        private const float bin_per_angle = 0.5f;
        private float radius => (Math.Max(DrawHeight, DrawWidth)) * scale;
        private float angleRange;
        private Container barsContainer;

        public PaddleDistributionGraph(IReadOnlyList<HitEvent> hitEvents, IBeatmap beatmap)
        {
            this.hitEvents = hitEvents.Where(e => !(e.HitObject.HitWindows is HitWindows.EmptyHitWindows) && e.HitObject is Beat && e.Result.IsHit()).ToList();

            angleRange = (float)BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.CircleSize, 75, 25, 10);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (hitEvents == null || hitEvents.Count == 0)
                return;

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
                    Height = Math.Max(0.05f, (float)bins[i] / maxCount),
                    Index = i
                };

            var paddedAngleRange = angleRange + (1 * 2); // 3° padding horizontally

            InternalChildren = new Drawable[]
            {
                barsContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
                new CircularContainer
                {
                    Masking = true,
                    BorderColour = Colour4.White,
                    BorderThickness = 2,
                    RelativeSizeAxes = Axes.Both,
                    FillAspectRatio = 1,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Scale = new Vector2(scale),
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.White.Opacity(0.25f)
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

            var barRadius = radius - (17 * scale);

            foreach (var bar in bars)
            {
                var pos = Extensions.GetCircularPosition(barRadius, (bar.Index * bin_per_angle) - (angleRange / 2));
                pos.Y += radius;

                barsContainer.Add(bar.With(b =>
                {
                    b.Position = pos;
                    b.Height /= 2;
                }));
            }
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

                Padding = new MarginPadding { Horizontal = 1 };

                InternalChild = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#66FFCC")
                };
            }
        }
    }
}
