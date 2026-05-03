using System;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Tau.Objects.Drawables;

public partial class DrawableHardBeatNestedHitObject : DrawableTauHitObject<HardBeatNestedHitObject>
{
    public new DrawableHardBeat ParentHitObject => (DrawableHardBeat)base.ParentHitObject;

    /// <summary>
    /// Lenience ms for the second key press.
    /// Has the same value as taiko.
    /// </summary>
    public const double HIT_WINDOW_MS = 30;

    public DrawableHardBeatNestedHitObject()
        : this(null)
    {
    }

    public DrawableHardBeatNestedHitObject([CanBeNull] HardBeatNestedHitObject obj)
        : base(obj)
    {
    }

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        if (!ParentHitObject.Result.HasResult)
            return;

        if (!ParentHitObject.Result.IsHit)
        {
            ApplyMinResult();
            return;
        }

        if (!userTriggered)
        {
            if (timeOffset - ParentHitObject.Result.TimeOffset > HIT_WINDOW_MS)
                ApplyMinResult();
            return;
        }

        if (Math.Abs(timeOffset - ParentHitObject.Result.TimeOffset) <= HIT_WINDOW_MS)
            ApplyMaxResult();
    }

    public override bool OnPressed(KeyBindingPressEvent<TauAction> e)
    {
        if (!ParentHitObject.IsHit)
            return false;

        if (ParentHitObject.HitAction == null)
            return false;

        if (!ParentHitObject.Actions.Contains(e.Action))
            return false;

        return UpdateResult(true);
    }

    public override void OnKilled()
    {
        base.OnKilled();

        if (!Judged && Time.Current > ParentHitObject?.HitObject.GetEndTime())
            ApplyMinResult();
    }
}
