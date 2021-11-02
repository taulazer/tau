using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Rulesets.Tau.Objects;
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

        protected override SelectionHandler<HitObject> CreateSelectionHandler() => new TauSelectionHandler();

        public override HitObjectSelectionBlueprint CreateHitObjectBlueprintFor(HitObject hitObject)
        {
            switch (hitObject)
            {
                case Beat beat:
                    return new BeatSelectionBlueprint(beat);

                case HardBeat hardBeat:
                    return new HardBeatSelectionBlueprint(hardBeat);

                case Slider slider:
                    return new SliderSelectionBlueprint(slider);
            }

            return base.CreateHitObjectBlueprintFor(hitObject);
        }
    }
}
