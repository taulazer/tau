using System;
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
    public class TauDrawableRuleset : DrawableRuleset<TauHitObject>
    {
        public TauDrawableRuleset(TauRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        [Cached]
        private TauCachedProperties properties { get; set; } = new();

        internal TauCachedProperties CachedProperties => properties;

        public override DrawableHitObject<TauHitObject> CreateDrawableRepresentation(TauHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new TauInputManager(Ruleset?.RulesetInfo);
        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TauFramedReplayInputHandler(replay);
        protected override ReplayRecorder CreateReplayRecorder(Score score) => new TauReplayRecorder(score);
        protected override ResumeOverlay CreateResumeOverlay() => new TauResumeOverlay();

        protected override Playfield CreatePlayfield() => new TauPlayfield();
        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TauPlayfieldAdjustmentContainer();

        public override void RequestResume(Action continueResume)
        {
            // We know we have a resume overlay, no need to check if we don't have one.

            ResumeOverlay.GameplayCursor = Cursor;
            ResumeOverlay.ResumeAction = continueResume;
            ResumeOverlay.Show();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            properties.SetRange(Beatmap.Difficulty.CircleSize);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            properties.Dispose();
        }
    }
}
