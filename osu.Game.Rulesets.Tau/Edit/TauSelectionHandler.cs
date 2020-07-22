using System.Linq;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Edit
{
    public class TauSelectionHandler : SelectionHandler
    {
        private Box debug1;
        private Box debug2;

        public override bool HandleMovement(MoveSelectionEvent moveEvent)
        {
            foreach (var h in SelectedHitObjects.OfType<TauHitObject>())
            {
                if (h is HardBeat)
                    continue;

                h.Angle = moveEvent.ScreenSpacePosition.GetDegreesFromPosition(ScreenSpaceDrawQuad.Centre);

                debug1.Position = moveEvent.ScreenSpacePosition;
                debug2.Position = ScreenSpaceDrawQuad.Centre;

                EditorBeatmap?.UpdateHitObject(h);
            }

            return true;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddInternal(debug1 = new Box
            {
                Size = new Vector2(16),
                Colour = Color4.Red
            });

            AddInternal(debug2 = new Box
            {
                Size = new Vector2(16),
                Colour = Color4.Blue
            });
        }
    }
}
