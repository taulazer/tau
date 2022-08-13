using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauCursor : GameplayCursorContainer
    {
        public readonly Paddle DrawablePaddle;
        public readonly List<Paddle> AdditionalPaddles = new();
        public IEnumerable<Paddle> AllPaddles => DrawablePaddle.Yield().Concat(AdditionalPaddles);

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

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load(IReadOnlyList<Mod> mods)
        {
            if (mods is null)
                return;

            rotationLock = mods.OfType<TauModRoundabout>().FirstOrDefault()?.Direction.Value;

            if (mods.GetMod(out TauModDual dual))
            {
                for (int i = 1; i < dual.PaddleCount.Value; i++)
                {
                    var paddle = new Paddle();
                    Add(paddle);
                    AdditionalPaddles.Add(paddle);
                }
            }
        }

        private float lastLockedRotation;
        private RotationDirection? rotationLock;

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            float prev = lastLockedRotation;
            float nextAngle = ScreenSpaceDrawQuad.Centre.GetDegreesFromPosition(e.ScreenSpaceMousePosition);
            AngleDistanceFromLastUpdate = Extensions.GetDeltaAngle(DrawablePaddle.Rotation, nextAngle);
            float diff = Extensions.GetDeltaAngle(nextAngle, prev);

            switch (rotationLock)
            {
                case RotationDirection.Clockwise:
                    lastLockedRotation = diff > 0 ? nextAngle : prev;
                    DrawablePaddle.Rotation = diff < 0 ? (lastLockedRotation - diff.LimitEase(40)) : lastLockedRotation;
                    break;

                case RotationDirection.Counterclockwise:
                    lastLockedRotation = diff < 0 ? nextAngle : prev;
                    DrawablePaddle.Rotation = diff > 0 ? (lastLockedRotation + diff.LimitEase(40)) : lastLockedRotation;
                    break;

                default:
                    DrawablePaddle.Rotation = lastLockedRotation = nextAngle;
                    break;
            }

            ActiveCursor.Position = ToLocalSpace(e.ScreenSpaceMousePosition);

            for (int i = 0; i < AdditionalPaddles.Count; i++)
            {
                AdditionalPaddles[i].Rotation = DrawablePaddle.Rotation + (360 / (AdditionalPaddles.Count + 1)) * (i + 1);
            }

            return false;
        }

        public override void Show()
        {
            this.FadeIn(250);
            DrawablePaddle.Show();

            foreach (var i in AdditionalPaddles)
            {
                i.Show();
            }
        }
    }
}
