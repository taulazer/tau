using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class SliderCircleOverlay : CompositeDrawable
{
    protected readonly BeatBlueprintPiece BeatPiece;

    private readonly Slider slider;
    private readonly SliderPosition position;

    [Resolved]
    private EditorClock clock { get; set; }

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
        float radius = TauPlayfield.BaseSize.X / 2;

        var circle = position == SliderPosition.Start ? (Beat)slider.HeadBeat : slider.EndBeat;

        BeatPiece.Position = Extensions.FromPolarCoordinates(-(float)(((circle.StartTime) - clock.Time.Current) / slider.TimePreempt * radius), -circle.Angle);
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

