using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Edit.Tools;

public class BeatCompositionTool : HitObjectCompositionTool
{
    public BeatCompositionTool()
        : base(nameof(Beat))
    {
    }

    public override Drawable CreateIcon() => new SpriteIcon
    {
        Icon = FontAwesome.Solid.Square
    };

    public override PlacementBlueprint CreatePlacementBlueprint() => new BeatPlacementBlueprint();
}
