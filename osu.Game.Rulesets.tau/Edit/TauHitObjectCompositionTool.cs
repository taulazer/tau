using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Tau.Edit.Blueprints;
using osu.Game.Rulesets.Tau.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauHitObjectCompositionTool : HitObjectCompositionTool
    {
        public TauHitObjectCompositionTool()
            : base(nameof(TauHitObject))
        {
        }

        public override PlacementBlueprint CreatePlacementBlueprint() => new TauHitObjectPlacementBlueprint();
    }
}
