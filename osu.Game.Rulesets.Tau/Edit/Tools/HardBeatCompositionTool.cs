using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;

namespace osu.Game.Rulesets.Tau.Edit.Tools;

public class HardBeatCompositionTool : HitObjectCompositionTool
{
    public HardBeatCompositionTool()
        : base("Hard Beat")
    {
    }

    public override Drawable CreateIcon() => new SpriteIcon
    {
        Icon = FontAwesome.Regular.Circle
    };

    public override PlacementBlueprint CreatePlacementBlueprint() => null;
}
