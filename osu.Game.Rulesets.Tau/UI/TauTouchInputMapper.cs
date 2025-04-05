#nullable enable

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Input.StateChanges;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI;

// basically copies OsuTouchInputMapper, but the first finger is always the "pointer"
internal partial class TauTouchInputMapper(TauInputManager inputManager) : Drawable
{
	private readonly List<TrackedTouch> trackedTouches = [];
	private TrackedTouch? positionTrackingTouch;

	// Required to handle touches outside of the playfield when screen scaling is enabled.
	public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

	protected override bool OnTouchDown(TouchDownEvent e)
	{
		var action = trackedTouches.Any(t => t.Action == TauAction.LeftButton)
			? TauAction.RightButton
			: TauAction.LeftButton;

		// Ignore any taps which trigger an action which is already handled. But track them for potential positional input in the future.
		bool shouldResultInAction = inputManager.AllowUserPresses && trackedTouches.All(t => t.Action != action);

		var newTouch = new TrackedTouch(e.Touch.Source, shouldResultInAction ? action : null);

		updatePositionTrackingTouch(newTouch);

		trackedTouches.Add(newTouch);

		// Important to update position before triggering the pressed action.
		handleTouchMovement(e);

		if (shouldResultInAction)
			inputManager.KeyBindingContainer.TriggerPressed(action);

		return true;
	}

	protected override void OnTouchMove(TouchMoveEvent e)
	{
		base.OnTouchMove(e);
		handleTouchMovement(e);
	}

	protected override void OnTouchUp(TouchUpEvent e)
	{
		var tracked = trackedTouches.Single(t => t.Source == e.Touch.Source);

		if (tracked.Action is TauAction action)
			inputManager.KeyBindingContainer.TriggerReleased(action);

		if (positionTrackingTouch == tracked)
			positionTrackingTouch = null;

		trackedTouches.Remove(tracked);

		base.OnTouchUp(e);
	}

	private void handleTouchMovement(TouchEvent touchEvent)
	{
		if (touchEvent is TouchMoveEvent moveEvent)
		{
			var trackedTouch = trackedTouches.Single(t => t.Source == touchEvent.Touch.Source);
			trackedTouch.DistanceTravelled += moveEvent.Delta.Length;
		}

		// Movement should only be tracked for the positionTrackingTouch.
		if (touchEvent.Touch.Source != positionTrackingTouch?.Source)
			return;

		if (!inputManager.AllowUserCursorMovement)
			return;

		new MousePositionAbsoluteInput { Position = touchEvent.ScreenSpaceTouch.Position }.Apply(inputManager.CurrentState, inputManager);
	}

	private void updatePositionTrackingTouch(TrackedTouch newTouch)
	{
		// We only want to use the new touch for position tracking if no other touch is tracking position yet.
		// This makes the first finger the "cursor", which makes tapping far-away bursts easier by using finger 1 to jump,
		// hitting the first note, while the others are used to alternating to hit the rest of the notes in the burst.
		if (positionTrackingTouch == null)
		{
			positionTrackingTouch = newTouch;
			return;
		}

		// In the case the new touch was not used for position tracking, we should also check the previous position tracking touch.
		// If it still has its action pressed, that action should be released.
		// This is done to allow tracking with the initial touch while still having both Left/Right actions available for alternating with two more touches.
		if (positionTrackingTouch.Action is TauAction touchAction)
		{
			inputManager.KeyBindingContainer.TriggerReleased(touchAction);
			positionTrackingTouch.Action = null;
		}
	}

	private class TrackedTouch(TouchSource source, TauAction? action)
	{
		public readonly TouchSource Source = source;
		public TauAction? Action = action;

		/// <summary>
		/// The total distance on screen travelled by this touch (in local pixels).
		/// </summary>
		public float DistanceTravelled;
	}
}
