using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class SliderNodePiece : BlueprintPiece<Slider>
{
    public Action<SliderNodePiece, MouseButtonEvent> RequestSelection;

    public Action<SliderNode> DragStarted;
    public Action<DragEvent> DragInProgress;
    public Action DragEnded;

    public BindableList<SliderNode> PointsInSegment;

    public readonly BindableBool IsSelected = new BindableBool();
    public readonly SliderNode SliderNode;

    private readonly Slider slider;
    private readonly Container marker;
    private readonly Drawable markerRing;

    [Resolved]
    private OsuColour colours { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    private IBindable<float> sliderAngle;

    public SliderNodePiece(Slider slider, SliderNode sliderNode)
    {
        this.slider = slider;
        SliderNode = sliderNode;
        // we don't want to run the path type update on construction as it may inadvertently change the slider.
        cachePoints(slider);
        slider.Path.Version.BindValueChanged(_ =>
        {
            cachePoints(slider);
        });

        sliderNode.Changed += updateMarkerDisplay;
        Origin = Anchor.Centre;
        AutoSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            marker = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Children = new[]
                {
                    new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(20), // TODO: Change size to smaller than default note size.
                    },
                    markerRing = new CircularContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(28), // TODO: Change size to smaller than default note size.
                        Masking = true,
                        BorderThickness = 2,
                        BorderColour = Color4.White,
                        Alpha = 0,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sliderAngle = slider.AngleBindable.GetBoundCopy();
        sliderAngle.BindValueChanged(_ => updateMarkerDisplay());
        IsSelected.BindValueChanged(_ => updateMarkerDisplay());

        updateMarkerDisplay();
    }


    // The connecting path is excluded from positional input
    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => marker.ReceivePositionalInputAt(screenSpacePos);

    protected override bool OnHover(HoverEvent e)
    {
        updateMarkerDisplay();
        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        updateMarkerDisplay();
    }

    // Used to pair up mouse down/drag events with their corresponding mouse up events,
    // to avoid deselecting the piece by accident when the mouse up corresponding to the mouse down/drag fires.
    private bool keepSelection;

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (RequestSelection == null)
            return false;

        switch (e.Button)
        {
            case MouseButton.Left:
                // if control is pressed, do not do anything as the user may be adding to current selection
                // or dragging all currently selected control points.
                // if it isn't and the user's intent is to deselect, deselection will happen on mouse up.
                if (e.ControlPressed && IsSelected.Value)
                    return true;

                RequestSelection.Invoke(this, e);
                keepSelection = true;

                return true;

            case MouseButton.Right:
                if (!IsSelected.Value)
                    RequestSelection.Invoke(this, e);

                keepSelection = true;
                return false; // Allow context menu to show
        }

        return false;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        base.OnMouseUp(e);

        // ctrl+click deselects this piece, but only if this event
        // wasn't immediately preceded by a matching mouse down or drag.
        if (IsSelected.Value && e.ControlPressed && !keepSelection)
            IsSelected.Value = false;

        keepSelection = false;
    }

    protected override bool OnClick(ClickEvent e) => RequestSelection != null;

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (RequestSelection == null)
            return false;

        if (e.Button == MouseButton.Left)
        {
            DragStarted?.Invoke(SliderNode);
            keepSelection = true;
            return true;
        }

        return false;
    }

    protected override void OnDrag(DragEvent e) => DragInProgress?.Invoke(e);

    protected override void OnDragEnd(DragEndEvent e) => DragEnded?.Invoke();

    private void cachePoints(Slider slider) => PointsInSegment = slider.Path.Nodes;

    /// <summary>
    /// Updates the state of the circular control point marker.
    /// </summary>
    private void updateMarkerDisplay()
    {
        float radius = TauPlayfield.BaseSize.X / 2;
        Position = Extensions.FromPolarCoordinates((float)(((SliderNode.Time + slider.StartTime) - clock.Time.Current) / slider.TimePreempt * radius), -SliderNode.Angle);

        markerRing.Alpha = IsSelected.Value ? 1 : 0;

        Color4 colour = colours.Gray1;

        if (IsHovered || IsSelected.Value)
            colour = colour.Lighten(1);

        marker.Colour = colour;
    }
}
