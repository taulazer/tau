using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableTauPool<T> : DrawablePool<T>
        where T : DrawableHitObject, new()
    {
        private readonly Func<DrawableTauHitObject, bool> checkValidation;
        private readonly Func<DrawableHitObject, double, bool> checkHittable;

        public DrawableTauPool(Func<DrawableTauHitObject, bool> checkValidation, Func<DrawableHitObject, double, bool> checkHittable, int initialSize, int? maximumSize = null)
            : base(initialSize, maximumSize)
        {
            this.checkValidation = checkValidation;
            this.checkHittable = checkHittable;
        }

        protected override T CreateNewDrawable() => base.CreateNewDrawable().With(o =>
        {
            if (o is DrawableTauHitObject tauHitObject)
            {
                tauHitObject.CheckValidation += checkValidation;
                tauHitObject.CheckHittable += checkHittable;
            }
        });
    }
}
