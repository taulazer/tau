using System.Linq;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauSelectionHandler : SelectionHandler
    {
        public override bool HandleMovement(MoveSelectionEvent moveEvent)
        {
            foreach (var h in EditorBeatmap.SelectedHitObjects.OfType<TauHitObject>())
            {
                if (h is HardBeat)
                    continue;

                h.Angle = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(moveEvent.ScreenSpacePosition);

                EditorBeatmap?.Update(h);
            }

            return true;
        }
    }
}
