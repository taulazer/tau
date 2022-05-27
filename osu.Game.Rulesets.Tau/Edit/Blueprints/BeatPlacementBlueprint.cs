using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osuTK.Graphics;
using Logger = osu.Framework.Logging.Logger;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints;

public class BeatPlacementBlueprint : PlacementBlueprint
{
    public Beat BeatObject => (Beat)base.HitObject;
    protected readonly Box Distance;
    private readonly BeatBlueprintPiece beatBlueprintPiece;

    public BeatPlacementBlueprint()
        : base(new Beat())
    {
        InternalChildren = new Drawable[]
        {
            beatBlueprintPiece = new BeatBlueprintPiece(),
            Distance = new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.Y,
                RelativePositionAxes = Axes.Both,
                Height = .5f,
                Width = 2.5f,
                Colour = Color4.Yellow.Opacity(.5f),
                EdgeSmoothness = Vector2.One
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BeginPlacement();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        BeatObject.Angle = beatBlueprintPiece.Rotation;
        EndPlacement(true);
        return true;
    }

    public override void UpdateTimeAndPosition(SnapResult result)
    {
        base.UpdateTimeAndPosition(result);
        float rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(result.ScreenSpacePosition);
        beatBlueprintPiece.Rotation = Distance.Rotation = rotation;
        // beatBlueprintPiece.Rotation = Distance.Rotation = BeatObject.Angle;

        beatBlueprintPiece.Position = ToLocalSpace(result.ScreenSpacePosition);
        Logger.Log(result.ScreenSpacePosition.ToString());
    }
}
