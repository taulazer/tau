// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.tau.Objects;
using osu.Game.Rulesets.tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        private TauCursorContainer cursor;
        protected override GameplayCursorContainer CreateCursor() => cursor;

        public TauPlayfield(Func<TauHitObject, DrawableHitObject<TauHitObject>> createDrawableRepresentation)
        {
            cursor = new TauCursorContainer(createDrawableRepresentation);

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
            });
        }
    }
}
