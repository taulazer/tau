using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit.Compose.Components;
using System.Collections.Generic;
using osu.Game.Rulesets.Tau.Edit.Tools.Big;
using osu.Game.Rulesets.Tau.Edit.Tools.Hard;
using osu.Game.Rulesets.Tau.Edit.Tools.Roll;
using osu.Game.Rulesets.Tau.Edit.Tools.Slider;
using osu.Game.Rulesets.Tau.Edit.Tools.Tap;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauHitObjectComposer : HitObjectComposer<TauHitObject>
    {
        public TauHitObjectComposer(Ruleset ruleset)
            : base(ruleset)
        {
        }

        protected override DrawableRuleset<TauHitObject> CreateDrawableRuleset(Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            => new DrawableTauEditRuleset((TauRuleset) ruleset, beatmap, mods);

        protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => new HitObjectCompositionTool[]
        {
            new TauHitBeatCompositionTool(),
            new TauSliderCompositionTool(),
            new TauHardBeatCompositionTool(),
            new TauBigBeatCompositionTool(),
            new TauRollBeatCompositionTool(),
        };

        protected override ComposeBlueprintContainer CreateBlueprintContainer() => new TauBlueprintContainer(HitObjects);
    }
}
