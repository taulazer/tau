// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        private TauCursor cursor;

        // Hides the cursor
        protected override GameplayCursorContainer CreateCursor() => null;

        public TauPlayfield()
        {
            cursor = new TauCursor();

            AddRangeInternal(new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.6f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new CircularProgress
                        {
                            RelativeSizeAxes = Axes.Both,
                            Current = new BindableDouble(1),
                            InnerRadius = 0.025f / 4,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fit,
                            FillAspectRatio = 1, // 1:1 Aspect ratio to get a perfect circle
                        }
                    }
                },
                HitObjectContainer,
                cursor
            });
        }
    }
}
