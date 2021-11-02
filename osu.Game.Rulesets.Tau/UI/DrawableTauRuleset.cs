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
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class DrawableTauRuleset : DrawableRuleset<TauHitObject>
    {
        public DrawableTauRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override ResumeOverlay CreateResumeOverlay() => new TauResumeOverlay(Beatmap.BeatmapInfo.BaseDifficulty);

        protected override Playfield CreatePlayfield() => new TauPlayfield(Beatmap.BeatmapInfo.BaseDifficulty);

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TauFramedReplayInputHandler(replay);

        public override DrawableHitObject<TauHitObject> CreateDrawableRepresentation(TauHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new TauInputManager(Ruleset?.RulesetInfo);

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer();

        protected override ReplayRecorder CreateReplayRecorder(Score score) => new TauReplayRecorder(score);
    }
}
