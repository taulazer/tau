// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
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
                                Current = new BindableDouble(0.1f),
                                InnerRadius = 0.05f,
                                Rotation = -360 * (0.1f / 2)
                            },
                            new EquilateralTriangle
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Y,
                                // todo: strange accuracy.. needs fix
                                Size = new Vector2(10f, 1f),
                                Scale = new Vector2(1f, 0.25f / 4.5f)
                            }
                        }
                    }
                };
            }

            protected override void Update()
            {
                base.Update();

                Rotation += (float)Time.Elapsed / 10;
            }
        }
    }
}
