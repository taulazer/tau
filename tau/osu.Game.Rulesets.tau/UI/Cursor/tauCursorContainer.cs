// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class TauCursorContainer : GameplayCursorContainer
    {
        private TauCursor cursor;
        protected override Drawable CreateCursor() => cursor;

        public TauCursorContainer(Func<TauHitObject, DrawableHitObject<TauHitObject>> createDrawableRepresentation)
        {
            cursor = new TauCursor(createDrawableRepresentation);
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            // Overrides mouse movement handling
            return false;
        }
    }
}
