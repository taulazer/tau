using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauDrawableRuleset : DrawableRuleset<TauHitObject>
    {
        public TauDrawableRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => new TauDependencyContainer(Beatmap, parent);

        public override DrawableHitObject<TauHitObject> CreateDrawableRepresentation(TauHitObject h)
            => h switch
            {
                Beat beat => new DrawableBeat(beat),
                _ => null
            };

        protected override PassThroughInputManager CreateInputManager() => new TauInputManager(Ruleset?.RulesetInfo);

        protected override Playfield CreatePlayfield() => new TauPlayfield();
        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer();
    }
}
