using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauHitObjectComposer : HitObjectComposer<TauHitObject>
    {
        public TauHitObjectComposer(Ruleset ruleset)
            : base(ruleset)
        {
        }

        protected override IReadOnlyList<HitObjectCompositionTool> CompositionTools => Array.Empty<HitObjectCompositionTool>();

        protected override ComposeBlueprintContainer CreateBlueprintContainer(IEnumerable<DrawableHitObject> hitObjects) =>
            new TauBlueprintContainer(hitObjects);
    }
}
