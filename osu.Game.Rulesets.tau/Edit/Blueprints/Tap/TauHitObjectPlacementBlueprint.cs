using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap.Components;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    public class TauHitObjectPlacementBlueprint : PlacementBlueprint
    {
        public new TauHitObject TauHit => (TauHitObject)HitObject;

        private readonly TauHitPiece hitPiece;

        //TODO: All instances of TauHitObject needs to be changed to the TapObject once HitObject is abstracted.
        public TauHitObjectPlacementBlueprint()
            : base(new TauHitObject()) => InternalChild = hitPiece = new TauHitPiece();

        protected override void Update()
        {
            base.Update();

            hitPiece.UpdateFrom(TauHit);
        }

        protected override bool OnClick(ClickEvent e)
        {
            EndPlacement(true);
            return true;
        }

        public override void UpdatePosition(Vector2 screenSpacePosition)
        {
            BeginPlacement();
            TauHit.PositionToEnd = ToLocalSpace(screenSpacePosition); // Need to fix this up
        }

    }
}
