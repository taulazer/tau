using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Skinning.Default
{
    public class HardBeatPiece : CircularContainer
    {
        [Resolved]
        private DrawableHitObject drawableObject { get; set; }

        public HardBeatPiece()
        {
            Masking = true;
            BorderThickness = 5;
            BorderColour = Color4.White;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            drawableObject.ApplyCustomUpdateState += updateState;
            updateState(drawableObject, drawableObject.State.Value);
        }

        private void updateState(DrawableHitObject drawableObject, ArmedState state)
        {
            const double time_fade_hit = 250, time_fade_miss = 400;

            using (BeginAbsoluteSequence(drawableObject.HitStateUpdateTime))
            {
                switch (state)
                {
                    case ArmedState.Hit:
                        this.TransformTo(nameof(BorderThickness), 0f, time_fade_hit);

                        break;

                    case ArmedState.Miss:
                        this.TransformTo(nameof(BorderThickness), 0f, time_fade_miss, Easing.OutQuint);

                        break;
                }
            }
        }
    }
}
