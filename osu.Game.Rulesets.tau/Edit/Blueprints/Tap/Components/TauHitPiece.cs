using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osu.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using osu.Game.Graphics;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Tap.Components
{
    public class TauHitPiece : BlueprintPiece<TauHitObject>
    {
        public TauHitPiece()
        {
            Origin = Anchor.Centre;

            Size = new Vector2(10);

            CornerRadius = Size.X / 2;
            CornerExponent = 2;

            InternalChild = new SquarePiece();
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Colour = colours.Yellow;
        }

        public override void UpdateFrom(TauHitObject hitObject)
        {
            base.UpdateFrom(hitObject);

            Scale = new Vector2(TauHitObject.SIZE);
        }

    }
}
