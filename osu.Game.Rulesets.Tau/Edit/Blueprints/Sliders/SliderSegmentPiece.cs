using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class SliderSegmentPiece : CompositeDrawable
{
    public readonly SliderNode SliderNode;

    private readonly DrawableSlider.SliderPath path;
    private readonly Slider slider;
    public int NodeIndex { get; set; }

    private IBindable<float> sliderAngle;
    private IBindable<int> pathVersion;

    [Resolved]
    private EditorClock clock { get; set; }

    public SliderSegmentPiece(Slider slider, int nodeIndex)
    {
        this.slider = slider;
        NodeIndex = nodeIndex;

        Origin = Anchor.Centre;
        AutoSizeAxes = Axes.Both;

        SliderNode = slider.Path.Nodes[nodeIndex];

        InternalChild = path = new DrawableSlider.SliderPath()
        {
            Anchor = Anchor.Centre,
            PathRadius = 1
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sliderAngle = slider.AngleBindable.GetBoundCopy();
        sliderAngle.BindValueChanged(_ => updateConnectingPath());

        pathVersion = slider.Path.Version.GetBoundCopy();
        pathVersion.BindValueChanged(_ => updateConnectingPath());

        updateConnectingPath();
    }

    /// <summary>
    /// Updates the path connecting this control point to the next one.
    /// </summary>
    private void updateConnectingPath()
    {
        float radius = TauPlayfield.BaseSize.X / 2;
        Position = Extensions.FromPolarCoordinates(-(float)(((SliderNode.Time + slider.StartTime) - clock.Time.Current) / slider.TimePreempt * radius), SliderNode.Angle);

        path.ClearVertices();

        int nextIndex = NodeIndex + 1;
        if (nextIndex == 0 || nextIndex >= slider.Path.Nodes.Count)
            return;

        path.AddVertex(Vector3.Zero);
        path.AddVertex(new Vector3(SliderNode.Time, SliderNode.Angle, 1));

        path.OriginPosition = path.PositionInBoundingBox(Vector2.Zero);
    }
}
