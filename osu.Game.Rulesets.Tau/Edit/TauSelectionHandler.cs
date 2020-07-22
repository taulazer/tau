using System.Linq;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauSelectionHandler : SelectionHandler
    {
        public override bool HandleMovement(MoveSelectionEvent moveEvent)
        {
            foreach (var h in SelectedHitObjects.OfType<TauHitObject>())
            {
                if (h is HardBeat)
                    continue;

                var angle = moveEvent.ScreenSpacePosition.GetDegreesFromPosition(ScreenSpaceDrawQuad.Centre);
                h.Angle = angle;

                EditorBeatmap?.UpdateHitObject(h);
            }

            return true;
        }
    }
}
