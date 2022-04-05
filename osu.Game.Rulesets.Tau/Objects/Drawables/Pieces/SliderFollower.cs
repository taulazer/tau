using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Game.Rulesets.Tau.Objects.Drawables.Pieces
{
    public class SliderFollower : Container
    {
        public BindableBool IsTracking = new();

        public bool Inversed;

        public SliderFollower()
        {
            CircularContainer container;

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            Child = container = new CircularContainer
            {
                Masking = true,
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(16),
                Y = -0.5f - 0.025f,
                Alpha = 0,
                AlwaysPresent = true,
                Child = new BeatPiece(),
            };

            IsTracking.BindValueChanged(t =>
            {
                container.FadeTo(t.NewValue ? 1f : 0f, 600, Easing.OutQuint);
                container.MoveToY(t.NewValue ? (Inversed ? -0.5f + 0.05f : -0.5f - 0.025f) : -0.5f, 600, Easing.OutQuint);
            }, true);
        }

        public override void ClearTransformsAfter(double time, bool propagateChildren = false, string targetMember = null)
        {
            // Consider the case of rewinding - children's transforms are handled internally, so propagating down
            // any further will cause weirdness with the Tracking bool below. Let's not propagate further at this point.
            base.ClearTransformsAfter(time, false, targetMember);
        }

        public override void ApplyTransformsAt(double time, bool propagateChildren = false)
        {
            // For the same reasons as above w.r.t rewinding, we shouldn't propagate to children here either.
            // ReSharper disable once RedundantArgumentDefaultValue - removing the "redundant" default value triggers BaseMethodCallWithDefaultParameter
            base.ApplyTransformsAt(time, false);
        }

        public void UpdateProgress(float angle)
        {
            Rotation = angle;
        }
    }
}
