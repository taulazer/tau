using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap;

namespace osu.Game.Rulesets.Tau.Edit.Tools.Tap
{
    public class TauHitBeatCompositionTool : HitObjectCompositionTool
    {
        public TauHitBeatCompositionTool()
            : base("Hit Beat")
        {
        }

        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
