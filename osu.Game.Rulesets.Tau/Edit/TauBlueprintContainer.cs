using System.Collections.Generic;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauBlueprintContainer : ComposeBlueprintContainer
    {
        public TauBlueprintContainer(IEnumerable<DrawableHitObject> hitObjects)
            : base(hitObjects)
        {
        }

        protected override SelectionHandler CreateSelectionHandler() => new TauSelectionHandler();

        public override OverlaySelectionBlueprint CreateBlueprintFor(DrawableHitObject hitObject) =>
            new TauSelectionBlueprint(hitObject);
    }
}
