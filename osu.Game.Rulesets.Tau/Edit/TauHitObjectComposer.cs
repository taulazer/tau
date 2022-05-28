using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Edit.Tools;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Components.TernaryButtons;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit;

public class TauHitObjectComposer : HitObjectComposer<TauHitObject>
{
    public TauHitObjectComposer(Ruleset ruleset)
        : base(ruleset)
    {
    }

    private readonly Bindable<TernaryState> angluarGridSnapToggle = new();
    private TauAngularPositionSnapGrid angularPositionSnapGrid;

    protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => new HitObjectCompositionTool[]
    {
        new BeatCompositionTool(),
        new SliderCompositionTool(),
        new HardBeatCompositionTool()
    };

    protected override IEnumerable<TernaryButton> CreateTernaryButtons() => base.CreateTernaryButtons().Concat(new[]
    {
        new TernaryButton(angluarGridSnapToggle, "Grid Snap", () => new SpriteIcon { Icon = FontAwesome.Solid.Th })
    });

    private Bindable<HitObject> placementObject;

    [BackgroundDependencyLoader]
    private void load()
    {
        LayerBelowRuleset.AddRange(new Drawable[]
        {
            // angularPositionSnapGrid = new TauAngularPositionSnapGrid
            // {
            //     RelativeSizeAxes = Axes.Both
            // }
        });
    }
}
