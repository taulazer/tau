using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints.Tap;

namespace osu.Game.Rulesets.Tau.Edit.Tools.Hard
{
    public class TauHardBeatCompositionTool : HitObjectCompositionTool
    {
        public TauHardBeatCompositionTool()
            : base("Hard Beat")
        {
        }

        //TODO: Change blueprint once Hard Beats are implemented.
        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
