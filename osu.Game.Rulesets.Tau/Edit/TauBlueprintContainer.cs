using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauBlueprintContainer : ComposeBlueprintContainer
    {
        public TauBlueprintContainer(HitObjectComposer composer)
            : base(composer)
        {
        }

        protected override SelectionHandler CreateSelectionHandler() => new TauSelectionHandler();

        public override OverlaySelectionBlueprint CreateBlueprintFor(DrawableHitObject hitObject)
        {
            switch (hitObject)
            {
                case DrawableBeat beat:
                    return new BeatSelectionBlueprint(beat);

                case DrawableHardBeat hardBeat:
                    return new HardBeatSelectionBlueprint(hardBeat);
            }

            return base.CreateBlueprintFor(hitObject);
        }
    }
}
