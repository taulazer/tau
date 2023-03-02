using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.StateChanges;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModAutopilot : Mod, IApplicableFailOverride, IUpdatableByPlayfield, IApplicableToDrawableRuleset<TauHitObject>
    {
        public override string Name => "Autopilot";
        public override string Acronym => "AP";
        public override IconUsage? Icon => OsuIcon.ModAutopilot;
        public override ModType Type => ModType.Automation;
        public override LocalisableString Description => ModStrings.AutopilotDescription;
        public override double ScoreMultiplier => 1;
        public override Type[] IncompatibleMods => new[] { typeof(ModRelax), typeof(ModFailCondition), typeof(ModNoFail), typeof(ModAutoplay) };

        public bool PerformFail() => false;

        public bool RestartOnFail => false;

        private TauInputManager inputManager;

        private IFrameStableClock gameplayClock;

        private List<TauReplayFrame> replayFrames;

        private int currentFrame;

        public void Update(Playfield playfield)
        {
            if (currentFrame == replayFrames.Count - 1) return;

            double time = gameplayClock.CurrentTime;

            // Very naive implementation of autopilot based on proximity to replay frames.
            // TODO: this needs to be based on user interactions to better match stable (pausing until judgement is registered).
            if (Math.Abs(replayFrames[currentFrame + 1].Time - time) <= Math.Abs(replayFrames[currentFrame].Time - time))
            {
                currentFrame++;
                new MousePositionAbsoluteInput { Position = playfield.ToScreenSpace(replayFrames[currentFrame].Position) }.Apply(inputManager.CurrentState,
                    inputManager);
            }
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<TauHitObject> drawableRuleset)
        {
            gameplayClock = drawableRuleset.FrameStableClock;

            // Grab the input manager to disable the user's cursor, and for future use
            inputManager = ((TauDrawableRuleset)drawableRuleset).KeyBindingInputManager;
            inputManager.AllowUserCursorMovement = false;

            // Generate the replay frames the cursor should follow
            replayFrames = new TauAutoGenerator(drawableRuleset.Beatmap, drawableRuleset.Mods).Generate().Frames.Cast<TauReplayFrame>().ToList();
        }
    }
}
