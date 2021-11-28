using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauSelectionHandler : EditorSelectionHandler
    {
        public override bool HandleMovement(MoveSelectionEvent<HitObject> moveEvent)
        {
            foreach (var h in EditorBeatmap.SelectedHitObjects.OfType<TauHitObject>())
            {
                if (h is HardBeat)
                    continue;

                var currentMousePos = moveEvent.Blueprint.ScreenSpaceSelectionPoint + moveEvent.ScreenSpaceDelta;

                h.Angle = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(currentMousePos);

                EditorBeatmap?.Update(h);
            }

            return true;
        }
    }
}
