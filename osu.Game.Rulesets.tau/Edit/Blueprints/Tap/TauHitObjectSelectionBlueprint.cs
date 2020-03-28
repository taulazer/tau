using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap.Components;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osuTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    public class TauHitObjectSelectionBlueprint : TauSelectionBlueprint<TauHitObject>
    {
        protected new DrawabletauHitObject DrawableObject => (DrawabletauHitObject) base.DrawableObject;

        protected readonly TauHitPiece TauHitPiece;

        public TauHitObjectSelectionBlueprint(DrawabletauHitObject drawabletauHit)
            : base(drawabletauHit)
        {
            InternalChild = TauHitPiece = new TauHitPiece();
        }

        protected override void Update()
        {
            base.Update();

            TauHitPiece.UpdateFrom(HitObject);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => DrawableObject.ReceivePositionalInputAt(screenSpacePos);

        public override Quad SelectionQuad => DrawableObject.ScreenSpaceDrawQuad;
    }
}
