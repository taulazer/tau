using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using osuTK.Graphics;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Rulesets.Tau.Objects.Drawables.Pieces
{
    public class SquarePiece : Container
    {
        public SquarePiece()
        {
            Size = new osuTK.Vector2(TauHitObject.SIZE);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChild = new Container
            {
                Masking = true,
                BorderThickness = 10,
                BorderColour = Color4.White,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        AlwaysPresent = true,
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            };
        }
    }
}
