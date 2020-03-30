using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints
{
    /// <summary>
    /// A piece of a selection or placement blueprint which visualises an <see cref="BlueprintPiece"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="BlueprintPiece"/> which this <see cref="BlueprintPiece{T}"/> visualises.</typeparam>
    public abstract class BlueprintPiece<T> : CompositeDrawable
        where T : TauHitObject
    {
        /// <summary>
        /// Updates this <see cref="BlueprintPiece{T}"/> using the properties of a <see cref="BlueprintPiece"/>.
        /// </summary>
        /// <param name="hitObject">The <see cref="BlueprintPiece"/> to reference properties from.</param>
        public virtual void UpdateFrom(T hitObject) => Position = hitObject.Position;
    }
}
