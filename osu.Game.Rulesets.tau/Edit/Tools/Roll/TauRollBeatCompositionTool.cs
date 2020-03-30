using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap;

namespace osu.Game.Rulesets.Tau.Edit.Tools.Roll
{
    public class TauRollBeatCompositionTool : HitObjectCompositionTool
    {
        public TauRollBeatCompositionTool()
            : base("Roll Beat")
        {
        }

        //TODO: Change blueprint once Roll Beats are implemented.
        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
