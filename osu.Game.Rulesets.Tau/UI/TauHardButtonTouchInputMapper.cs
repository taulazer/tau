using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI;

// basically copies CatchTouchInputMapper, without the right side buttons
public partial class TauHardButtonTouchInputMapper : VisibilityContainer
{
	public override bool PropagatePositionalInputSubTree => true;
	public override bool PropagateNonPositionalInputSubTree => true;

	private readonly Dictionary<object, TauAction> trackedActionSources = [];

	private readonly KeyBindingContainer<TauAction> keyBindingContainer;

	private Container mainContent = null!;

	private InputArea hardButton1 = null!;
	private InputArea hardButton2 = null!;

	public TauHardButtonTouchInputMapper(TauInputManager inputManager)
	{
		keyBindingContainer = inputManager.KeyBindingContainer;
	}

	[BackgroundDependencyLoader]
	private void load(OsuColour colours)
	{
		const float width = 0.15f;
		// Ratio between normal move area height and total input height
		const float normal_area_height_ratio = 0.45f;

		RelativeSizeAxes = Axes.Both;

		// not sure how to make this a GridContainer, leaving as is since it works
		Child = mainContent = new Container
		{
			RelativeSizeAxes = Axes.Both,
			Alpha = 0,
			Child = new Container
			{
				RelativeSizeAxes = Axes.Both,
				Width = width,
				Children =
				[
					hardButton1 = new InputArea(TauAction.HardButton1, trackedActionSources)
					{
						RelativeSizeAxes = Axes.Both,
						Height = normal_area_height_ratio,
						Colour = colours.Gray9,
						Anchor = Anchor.BottomRight,
						Origin = Anchor.BottomRight,
					},
					hardButton2 = new InputArea(TauAction.HardButton2, trackedActionSources)
					{
						RelativeSizeAxes = Axes.Both,
						Height = 1 - normal_area_height_ratio,
					},
				]
			},
		};
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		Hide();
		return false;
	}

	protected override bool OnTouchDown(TouchDownEvent e)
	{
		return updateAction(e.Touch.Source, getTauActionFromInput(e.ScreenSpaceTouch.Position));
	}

	protected override void OnTouchMove(TouchMoveEvent e)
	{
		updateAction(e.Touch.Source, getTauActionFromInput(e.ScreenSpaceTouch.Position));
		base.OnTouchMove(e);
	}

	protected override void OnTouchUp(TouchUpEvent e)
	{
		updateAction(e.Touch.Source, null);
		base.OnTouchUp(e);
	}

	private bool updateAction(object source, TauAction? newAction)
	{
		TauAction? actionBefore = null;

		if (trackedActionSources.TryGetValue(source, out TauAction found))
			actionBefore = found;

		if (actionBefore != newAction)
		{
			if (newAction != null)
				trackedActionSources[source] = newAction.Value;
			else
				trackedActionSources.Remove(source);

			updatePressedActions();
		}

		return newAction != null;
	}

	private void updatePressedActions()
	{
		Show();

		if (trackedActionSources.ContainsValue(TauAction.HardButton1))
			keyBindingContainer.TriggerPressed(TauAction.HardButton1);
		else
			keyBindingContainer.TriggerReleased(TauAction.HardButton1);

		if (trackedActionSources.ContainsValue(TauAction.HardButton2))
			keyBindingContainer.TriggerPressed(TauAction.HardButton2);
		else
			keyBindingContainer.TriggerReleased(TauAction.HardButton2);
	}

	private TauAction? getTauActionFromInput(Vector2 screenSpaceInputPosition)
	{
		if (hardButton1.Contains(screenSpaceInputPosition))
			return TauAction.HardButton1;
		if (hardButton2.Contains(screenSpaceInputPosition))
			return TauAction.HardButton2;
		return null;
	}

	protected override void PopIn() => mainContent.FadeIn(300, Easing.OutQuint);
	protected override void PopOut() => mainContent.FadeOut(300, Easing.OutQuint);

	private partial class InputArea : CompositeDrawable, IKeyBindingHandler<TauAction>
	{
		private readonly TauAction handledAction;
		private readonly Box highlightOverlay;
		private readonly IEnumerable<KeyValuePair<object, TauAction>> trackedActions;
		private bool isHighlighted;

		public InputArea(TauAction handledAction, IEnumerable<KeyValuePair<object, TauAction>> trackedActions)
		{
			this.handledAction = handledAction;
			this.trackedActions = trackedActions;

			InternalChild = new Container
			{
				RelativeSizeAxes = Axes.Both,
				Masking = true,
				CornerRadius = 10,
				Children =
				[
					new Box
					{
						RelativeSizeAxes = Axes.Both,
						Alpha = 0.15f,
					},
					highlightOverlay = new Box
					{
						RelativeSizeAxes = Axes.Both,
						Alpha = 0,
						Blending = BlendingParameters.Additive,
					}
				]
			};
		}

		public bool OnPressed(KeyBindingPressEvent<TauAction> _)
		{
			updateHighlight();
			return false;
		}

		public void OnReleased(KeyBindingReleaseEvent<TauAction> _)
		{
			updateHighlight();
		}

		private void updateHighlight()
		{
			bool isHandling = trackedActions.Any(a => a.Value == handledAction);

			if (isHandling == isHighlighted)
				return;

			isHighlighted = isHandling;
			highlightOverlay.FadeTo(isHighlighted ? 0.1f : 0, isHighlighted ? 80 : 400, Easing.OutQuint);
		}
	}
}
