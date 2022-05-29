using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class SliderCircleOverlay : CompositeDrawable
{
    protected readonly BeatBlueprintPiece BeatPiece;

    private readonly Slider slider;
    private readonly SliderPosition position;

    public SliderCircleOverlay(Slider slider, SliderPosition position)
    {
        this.slider = slider;
        this.position = position;

        InternalChildren = new Drawable[]
        {
            BeatPiece = new BeatBlueprintPiece(),
        };
    }

    protected override void Update()
    {
        base.Update();

        var circle = position == SliderPosition.Start ? (Beat)slider.HeadBeat : slider.EndBeat;

        BeatPiece.UpdateFrom(circle);
    }

    public override void Hide()
    {
        BeatPiece.Hide();
    }

    public override void Show()
    {
        BeatPiece.Show();
    }
}

