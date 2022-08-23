using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables.Pieces
{
    public class StrictHardBeatPiece : CircularProgress
    {
        public BindableFloat NoteSize = new(16f);
        public BindableDouble AngleRange = new(25 * 0.75);

        public StrictHardBeatPiece()
        {
            Colour = Color4.White;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.Zero;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            FillAspectRatio = 1;
            FillMode = FillMode.Fit;
            InnerRadius = 1f;

            AngleRange.BindValueChanged(val =>
            {
                Current.Value = val.NewValue / 360;
                Rotation = -(float)(val.NewValue / 2);
            }, true);
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
