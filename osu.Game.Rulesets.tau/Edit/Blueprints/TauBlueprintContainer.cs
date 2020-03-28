using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Screens.Edit.Compose.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    public class TauBlueprintContainer : ComposeBlueprintContainer
    {
        public TauBlueprintContainer(IEnumerable<DrawableHitObject> drawableHitObjects)
            : base(drawableHitObjects)
        {
        }

        protected override SelectionHandler CreateSelectionHandler() => new TauSelectionHandler();

        public override OverlaySelectionBlueprint CreateBlueprintFor(DrawableHitObject hitObject)
        {
            switch (hitObject)
            {
                case DrawabletauHitObject tap:
                    return new TauHitObjectSelectionBlueprint(tap);
            }

            return base.CreateBlueprintFor(hitObject);
        }
    }
}
