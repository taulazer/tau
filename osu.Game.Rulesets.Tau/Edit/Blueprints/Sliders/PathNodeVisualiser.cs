using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.Sliders;

public class PathNodeVisualiser : CompositeDrawable, IKeyBindingHandler<PlatformAction>, IHasContextMenu
{
    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true; // allow context menu to appear outside of the playfield.
    internal readonly Container<SliderNodePiece> Pieces;
    internal readonly Container<SliderSegmentPiece> Connections;
    private readonly IBindableList<SliderNode> sliderNodes = new BindableList<SliderNode>();
    private readonly Slider slider;
    private readonly bool allowSelection;

    private InputManager inputManager;

    public Action<List<SliderNode>> RemoveControlPointsRequested;

    public PathNodeVisualiser(Slider slider, bool allowSelection)
    {
        this.slider = slider;
        this.allowSelection = allowSelection;

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            Connections = new Container<SliderSegmentPiece> { RelativeSizeAxes = Axes.Both },
            Pieces = new Container<SliderNodePiece> { RelativeSizeAxes = Axes.Both }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();

        sliderNodes.CollectionChanged += onNodesChanged;
        sliderNodes.BindTo(slider.Path.Nodes);
    }

    /// <summary>
    /// Selects the <see cref="SliderNodePiece"/> corresponding to the given <paramref name="sliderNode"/>,
    /// and deselects all other <see cref="SliderNodePiece"/>s.
    /// </summary>
    public void SetSelectionTo(SliderNode sliderNode)
    {
        foreach (var p in Pieces)
            p.IsSelected.Value = Equals(p.SliderNode, sliderNode);
    }

    private void onNodesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                // If inserting in the path (not appending),
                // update indices of existing connections after insert location
                if (e.NewStartingIndex < Pieces.Count)
                {
                    foreach (var connection in Connections)
                    {
                        if (connection.NodeIndex >= e.NewStartingIndex)
                            connection.NodeIndex += e.NewItems.Count;
                    }
                }

                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    var point = (SliderNode)e.NewItems[i];

                    Pieces.Add(new SliderNodePiece(slider, point).With(d =>
                    {
                        if (allowSelection)
                            d.RequestSelection = selectionRequested;

                        // d.DragStarted = dragStarted;
                        // d.DragInProgress = dragInProgress;
                        // d.DragEnded = dragEnded;
                    }));

                    Connections.Add(new SliderSegmentPiece(slider, e.NewStartingIndex + i));
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var point in e.OldItems.Cast<SliderNode>())
                {
                    Pieces.RemoveAll(p => Equals(p.SliderNode, point));
                    Connections.RemoveAll(c => Equals(c.SliderNode, point));
                }

                // If removing before the end of the path,
                // update indices of connections after remove location
                if (e.OldStartingIndex < Pieces.Count)
                {
                    foreach (var connection in Connections)
                    {
                        if (connection.NodeIndex >= e.OldStartingIndex)
                            connection.NodeIndex -= e.OldItems.Count;
                    }
                }

                break;
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Pieces.Any(piece => piece.IsHovered))
            return false;

        foreach (var piece in Pieces)
        {
            piece.IsSelected.Value = false;
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            // case PlatformAction.Delete:
            //     return DeleteSelected();
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }

    private void selectionRequested(SliderNodePiece piece, MouseButtonEvent e)
    {
        if (e.Button == MouseButton.Left && inputManager.CurrentState.Keyboard.ControlPressed)
            piece.IsSelected.Toggle();
        else
            SetSelectionTo(piece.SliderNode);
    }

    [Resolved(CanBeNull = true)]
    private IEditorChangeHandler changeHandler { get; set; }

    public MenuItem[] ContextMenuItems { get; }
}
