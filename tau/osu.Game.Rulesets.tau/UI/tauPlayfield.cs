// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI.Cursor;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI
{
    [Cached]
    public class TauPlayfield : Playfield
    {
        private TauCursor cursor;
        private JudgementContainer<DrawableTauJudgement> judgementLayer;

        // Hides the cursor
        protected override GameplayCursorContainer CreateCursor() => null;

        public TauPlayfield()
        {
            cursor = new TauCursor();

            AddRangeInternal(new Drawable[]
            {
                judgementLayer = new JudgementContainer<DrawableTauJudgement>
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.6f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new CircularProgress
                        {
                            RelativeSizeAxes = Axes.Both,
                            Current = new BindableDouble(1),
                            InnerRadius = 0.025f / 4,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fit,
                            FillAspectRatio = 1, // 1:1 Aspect ratio to get a perfect circle
                        }
                    }
                },
                HitObjectContainer,
                cursor
            });
        }

        public bool CheckIfWeCanValidate(DrawabletauHitObject obj) => cursor.CheckForValidation(obj);

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            var obj = (DrawabletauHitObject)h;
            obj.CheckValidation = CheckIfWeCanValidate;

            obj.OnNewResult += onNewResult;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            var tauObj = (DrawabletauHitObject)judgedObject;

            var b = tauObj.HitObject.PositionToEnd.GetDegreesFromPosition(tauObj.Box.AnchorPosition) * 4;
            var a = b *= (float)(Math.PI / 180);

            DrawableTauJudgement explosion = new DrawableTauJudgement(result, tauObj)
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Position = new Vector2(-(285 * (float)Math.Cos(a)), -(285 * (float)Math.Sin(a))),
                Rotation = tauObj.Box.Rotation + 90,
            };

            judgementLayer.Add(explosion);
        }
    }
}
