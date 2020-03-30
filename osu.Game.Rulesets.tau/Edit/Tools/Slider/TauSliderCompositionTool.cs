using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap;

namespace osu.Game.Rulesets.Tau.Edit.Tools.Slider
{
    public class TauSliderCompositionTool : HitObjectCompositionTool
    {
        public TauSliderCompositionTool()
            : base("Slider")
        {
        }

        //TODO: Change blueprint once Slider is implemented.
        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
