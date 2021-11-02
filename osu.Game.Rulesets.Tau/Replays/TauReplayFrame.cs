using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using osuTK;

namespace osu.Game.Rulesets.Tau.Replays
{
    public class TauReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public List<TauAction> Actions = new List<TauAction>();
        public Vector2 Position;

        public TauReplayFrame()
        {
        }

        public TauReplayFrame(double time, Vector2 position, params TauAction[] actions)
            : base(time)
        {
            Position = position;
            Actions.AddRange(actions);
        }

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            Position = currentFrame.Position;

            if (currentFrame.MouseLeft1) Actions.Add(TauAction.LeftButton);
            if (currentFrame.MouseRight1) Actions.Add(TauAction.RightButton);
            if (currentFrame.MouseLeft2) Actions.Add(TauAction.HardButton1);
            if (currentFrame.MouseRight2) Actions.Add(TauAction.HardButton2);
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(TauAction.LeftButton)) state |= ReplayButtonState.Left1;
            if (Actions.Contains(TauAction.RightButton)) state |= ReplayButtonState.Right1;
            if (Actions.Contains(TauAction.HardButton1)) state |= ReplayButtonState.Left2;
            if (Actions.Contains(TauAction.HardButton2)) state |= ReplayButtonState.Right2;

            return new LegacyReplayFrame(Time, Position.X, Position.Y, state);
        }
    }
}
