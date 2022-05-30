using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Compose;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class SliderSelectionBlueprint : TauSelectionBlueprint<Slider>
{
    protected new DrawableSlider DrawableObject => (DrawableSlider)base.DrawableObject;

    //protected SliderBodyPiece BodyPiece { get; private set; }
    protected SliderCircleOverlay HeadOverlay { get; private set; }
    protected SliderCircleOverlay TailOverlay { get; private set; }

    [CanBeNull]
    protected PathNodeVisualiser NodeVisualiser { get; private set; }

    [Resolved(CanBeNull = true)]
    private IDistanceSnapProvider snapProvider { get; set; }

    [Resolved(CanBeNull = true)]
    private IPlacementHandler placementHandler { get; set; }

    [Resolved(CanBeNull = true)]
    private IEditorChangeHandler changeHandler { get; set; }

    [Resolved(CanBeNull = true)]
    private EditorBeatmap editorBeatmap { get; set; }

    [Resolved(CanBeNull = true)]
    private BindableBeatDivisor beatDivisor { get; set; }

    public override Quad SelectionQuad => DrawableObject.ScreenSpaceDrawQuad;

    private readonly BindableList<SliderNode> sliderNodes = new BindableList<SliderNode>();
    private readonly IBindable<int> pathVersion = new Bindable<int>();
    private Vector2 rightClickPosition;

    [CanBeNull]
    private SliderNode placementNode;

    public SliderSelectionBlueprint(Slider hitObject)
        : base(hitObject)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            //BodyPiece = new SliderBodyPiece(),
            HeadOverlay = CreateCircleOverlay(HitObject, SliderPosition.Start),
            TailOverlay = CreateCircleOverlay(HitObject, SliderPosition.End),
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sliderNodes.BindTo(HitObject.Path.Nodes);

        pathVersion.BindTo(HitObject.Path.Version);
        pathVersion.BindValueChanged(_ => editorBeatmap?.Update(HitObject));

       // BodyPiece.UpdateFrom(HitObject);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        switch (e.Button)
        {
            case MouseButton.Right:
                rightClickPosition = e.MouseDownPosition;
                return false; // Allow right click to be handled by context menu

            case MouseButton.Left:
                // if (e.ControlPressed && IsSelected)
                // {
                //     changeHandler?.BeginChange();
                //     placementNode = addNode(e.MousePosition);
                //     NodeVisualiser?.SetSelectionTo(placementNode);
                //     return true; // Stop input from being handled and modifying the selection
                // }

                break;
        }

        return false;
    }

    // private SliderNode addNode(Vector2 position)
    // {
    //     position -= HitObject.Position;
    //
    //     int insertionIndex = 0;
    //     float minDistance = float.MaxValue;
    //
    //     for (int i = 0; i < sliderNodes.Count - 1; i++)
    //     {
    //         float dist = new Line(sliderNodes[i].Angle, sliderNodes[i + 1].Angle).DistanceToPoint(position);
    //
    //         if (dist < minDistance)
    //         {
    //             insertionIndex = i + 1;
    //             minDistance = dist;
    //         }
    //     }
    //
    //     var pathControlPoint = new SliderNode { Angle = TauPlayfield.Empty().Position.GetDegreesFromPosition() };
    //
    //     // Move the control points from the insertion index onwards to make room for the insertion
    //     controlPoints.Insert(insertionIndex, pathControlPoint);
    //
    //     HitObject.SnapTo(snapProvider);
    //
    //     return pathControlPoint;
    // }

    // Always refer to the drawable object's slider body so subsequent movement deltas are calculated with updated positions.
    public override Vector2 ScreenSpaceSelectionPoint => DrawableObject.ToScreenSpace(DrawableObject.Position);

    protected override void OnSelected()
    {
        AddInternal(NodeVisualiser = new PathNodeVisualiser(HitObject, true)
        {
            //RemoveControlPointsRequested = removeNodes
        });

        base.OnSelected();
    }

    protected virtual SliderCircleOverlay CreateCircleOverlay(Slider slider, SliderPosition position) => new(slider, position);
}

public enum SliderPosition
{
    Start,
    End
}
