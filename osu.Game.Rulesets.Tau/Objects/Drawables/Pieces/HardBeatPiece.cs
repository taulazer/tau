using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Tau.Configuration;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables.Pieces
{
    public partial class HardBeatPiece : CircularContainer
    {
        public BindableFloat NoteSize = new(16f);
        public BindableBool HighlightHardBeats = new(false);

        public HardBeatPiece()
        {
            Masking = true;
            BorderThickness = 5;
            BorderColour = HighlightHardBeats.Value ? Color4.Orange : Color4.White;
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            FillAspectRatio = 1;
            FillMode = FillMode.Fit;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true
            };

            NoteSize.BindValueChanged(value => BorderThickness = convertNoteSizeToThickness(value.NewValue));
            HighlightHardBeats.BindValueChanged(value => BorderColour = value.NewValue ? Color4.Orange : Color4.White);
        }

        [BackgroundDependencyLoader]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.HighlightHardBeats, HighlightHardBeats);
        }

        private float convertNoteSizeToThickness(float noteSize)
            => Interpolation.ValueAt(noteSize, 3f, 15f, 10f, 25f);
    }
}
