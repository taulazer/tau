using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;

namespace osu.Game.Rulesets.Tau.Edit.Tools;

public class SliderCompositionTool : HitObjectCompositionTool
{
    public SliderCompositionTool()
        : base("Slider")
    {
    }

    public override Drawable CreateIcon() => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Sliders);

    public override PlacementBlueprint CreatePlacementBlueprint() => null;
}
