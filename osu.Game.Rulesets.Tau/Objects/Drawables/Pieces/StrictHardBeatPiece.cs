using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables.Pieces
{
    public partial class StrictHardBeatPiece : CircularProgress
    {
        public BindableFloat NoteSize = new(16f);
        public BindableDouble AngleRange = new(25 * 0.75);

        public StrictHardBeatPiece()
        {
            Colour = Color4.White;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            FillAspectRatio = 1;
            FillMode = FillMode.Fit;
            InnerRadius = 1f;

            AngleRange.BindValueChanged(val =>
            {
                Progress = val.NewValue / 360;
                Rotation = -(float)(val.NewValue / 2);
            }, true);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            // Size is set to zero here as to avoid standard fallback to original sprite's size (1,1),
            // see: https://github.com/ppy/osu-framework/blob/603e15fb2e68826e55878dfc09e1d7414b7cdf90/osu.Framework/Graphics/Sprites/Sprite.cs#L181-L182
            Size = Vector2.Zero;
        }

        private float toNormalized(float value)
            => value / DrawWidth;

        private float convertNoteSizeToThickness(float noteSize)
            => Interpolation.ValueAt(noteSize, toNormalized(20f), toNormalized(50f), 10f, 25f);

        protected override void Update()
        {
            base.Update();

            if (!IsLoaded || NoteSize.Value == 0 || DrawWidth == 0)
                return;

            InnerRadius = convertNoteSizeToThickness(NoteSize.Value);
        }
    }
}
