using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauDrawableRuleset : DrawableRuleset<TauHitObject>
    {
        internal TauDependencyContainer TauDependencyContainer;

        public TauDrawableRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = base.CreateChildDependencies(parent);
            return TauDependencyContainer = new TauDependencyContainer(Beatmap, dependencies);
        }

        protected override void Dispose ( bool isDisposing ) {
            base.Dispose( isDisposing );
            TauDependencyContainer.Dispose();
        }

        public override DrawableHitObject<TauHitObject> CreateDrawableRepresentation(TauHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new TauInputManager(Ruleset?.RulesetInfo);
        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TauFramedReplayInputHandler(replay);
        protected override ReplayRecorder CreateReplayRecorder(Score score) => new TauReplayRecorder(score);

        protected override Playfield CreatePlayfield() => new TauPlayfield();
        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer();
    }
}
