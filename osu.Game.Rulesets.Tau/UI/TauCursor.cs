using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauCursor : GameplayCursorContainer
    {
        public readonly Paddle DrawablePaddle;

        public float AngleDistanceFromLastUpdate { get; private set; }

        protected override Drawable CreateCursor() => new AbsoluteCursor();

        public TauCursor()
        {
            FillAspectRatio = 1;
            FillMode = FillMode.Fit;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Add(DrawablePaddle = new Paddle());
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            var rotation = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);
            AngleDistanceFromLastUpdate = Extensions.GetDeltaAngle(DrawablePaddle.Rotation, rotation);
            DrawablePaddle.Rotation = rotation;
            ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);

            return false;
        }
    }
}
