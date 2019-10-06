// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osuTK;

namespace osu.Game.Rulesets.tau.UI.Cursor
{
    public class TauCursor : CompositeDrawable
    {
        private readonly IBindable<WorkingBeatmap> beatmap = new Bindable<WorkingBeatmap>();

        public TauCursor()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap)
        {
            InternalChild = new DefaultCursor(beatmap.Value.BeatmapInfo.BaseDifficulty.CircleSize);

            this.beatmap.BindTo(beatmap);
        }

        private class DefaultCursor : CompositeDrawable
        {
            public DefaultCursor(float cs = 5f)
            {
                RelativeSizeAxes = Axes.Both;

                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                FillMode = FillMode.Fit;
                FillAspectRatio = 1; // 1:1 Aspect Ratio.

                InternalChildren = new Drawable[]
                {
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(0.75f),
                        Children = new Drawable[]
                        {
                            new CircularProgress
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Current = new BindableDouble(convertValue(cs)),
                                InnerRadius = 0.05f,
                                Rotation = -360 * ((float)convertValue(cs) / 2)
                            },
                            new Box
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Y,
                                Size = new Vector2(5f, 0.5f),
                            }
                        }
                    }
                };

                const double a = 1;
                const double b = 10;
                const double c = 0.2;
                const double d = 0.005;

                // Thank you AlFas for this code.
                double convertValue(double value) => c + (d - c) * (value - a) / (b - a);
            }

            protected override bool OnMouseMove(MouseMoveEvent e)
            {
                var angle = Math.Atan2(OriginPosition.Y + Height / 2 - e.MousePosition.Y,
                                OriginPosition.X + Width / 2 - e.MousePosition.X) * 180.0 / Math.PI - 90;

                Rotation = (float)angle;

                return base.OnMouseMove(e);
            }
        }
    }
}
