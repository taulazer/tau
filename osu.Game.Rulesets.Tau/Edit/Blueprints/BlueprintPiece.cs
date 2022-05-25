using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints;

/// <summary>
/// A piece of a selection or placement blueprint which visualises an <see cref="TauHitObject"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="TauHitObject"/> which this <see cref="BlueprintPiece{T}"/> visualises.</typeparam>
public abstract class BlueprintPiece<T> : CompositeDrawable
    where T : TauHitObject
{
    public void UpdateFrom(T hitObject)
    {
    }
}
