using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override void LoadComplete()
        {
            base.LoadComplete();

            using (BeginDelayedSequence(50))
                Show();
        }


        [BackgroundDependencyLoader]
        private void load ( IReadOnlyList<Mod> mods ) {
            rotationLock = mods.OfType<TauModRoundabout>().FirstOrDefault()?.Direction.Value;
        }

        /// <summary>
        /// Eases a value towards a limit as it apprroaches infinity
        /// </summary>
        /// <param name="value">The parameter in range of [0, ∞)</param>
        /// <param name="limit">The returned value as <paramref name="value"/> approaches ∞</param>
        /// <param name="exponent">How fast the return value approaches the limit</param>
        float limitEase ( float value, float limit, float exponent = 1.0116194403f /* rougly equal to 2^(1/60) */ ) {
            return limit * ( 1 - 1 / MathF.Pow( exponent, Math.Abs( value ) ) );
        }

        float lastLockedRotation;
        RotationDirection? rotationLock;
        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            var prev = lastLockedRotation;
            var nextAngle = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition( e.ScreenSpaceMousePosition );
            var diff = Extensions.GetDeltaAngle( nextAngle, prev );
            switch ( rotationLock ) {
                case RotationDirection.Clockwise:
                    lastLockedRotation = diff > 0 ? nextAngle : prev;
                    DrawablePaddle.Rotation = diff < 0 ? ( lastLockedRotation - limitEase( diff, 40 ) ) : lastLockedRotation;
                    break;

                case RotationDirection.Counterclockwise:
                    lastLockedRotation = diff < 0 ? nextAngle : prev;
                    DrawablePaddle.Rotation = diff > 0 ? ( lastLockedRotation + limitEase( diff, 40 ) ) : lastLockedRotation;
                    break;

                default:
                    DrawablePaddle.Rotation = lastLockedRotation = nextAngle;
                    break;
            }

            ActiveCursor.Position = ToLocalSpace( e.ScreenSpaceMousePosition );

            return false;
        }

        public override void Show()
        {
            this.FadeIn(250);
            DrawablePaddle.Show();
        }
    }
}
