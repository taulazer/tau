using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap;

namespace osu.Game.Rulesets.Tau.Edit.Tools.Big
{
    public class TauBigBeatCompositionTool : HitObjectCompositionTool
    {
        public TauBigBeatCompositionTool()
            : base("Big Beat")
        {
        }

        //TODO: Change blueprint once Big Beats are implemented.
        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
