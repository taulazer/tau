using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints;

public class BeatPlacementBlueprint : PlacementBlueprint
{
    public Beat BeatObject => (Beat)base.HitObject;
    private readonly BeatBlueprintPiece beatBlueprintPiece;

    public BeatPlacementBlueprint()
        : base(new Beat())
    {
        InternalChild = beatBlueprintPiece = new BeatBlueprintPiece();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BeginPlacement();
    }

    protected override void Update()
    {
        base.Update();
        beatBlueprintPiece.UpdateFrom(BeatObject);
    }
}
