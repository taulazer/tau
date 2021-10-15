using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Rulesets.Tau.UI.Cursor
{
    public class TriangleExplosion : Container
    {
        private readonly List<Triangle> tris = new List<Triangle>();

        public TriangleExplosion(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                tris.Add(new Triangle
                {
                    Rotation = RNG.NextSingle(360f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.BottomCentre,
                    Size = new Vector2(RNG.NextSingle(5f, 25f)),
                    Colour = TauPlayfield.ACCENT_COLOR.Value,
                    Blending = BlendingParameters.Additive,
                    Alpha = 0,
                });
            }

            AddRange(tris);
        }

        public override void Show()
        {
            const float duration = 1500;

            foreach (var triangle in tris)
            {
                triangle.FadeTo(RNG.NextSingle(0.25f, 1.0f))
                        .MoveTo(Extensions.GetCircularPosition(RNG.NextSingle(20, 100f), RNG.NextSingle(-30, 30)), duration, Easing.OutExpo)
                        .ResizeTo(RNG.NextSingle(1f, 10f), duration, Easing.OutQuint)
                        .FadeOut(duration, Easing.OutQuint);

                triangle.Expire(true);
            }

            this.Delay(duration).Expire(true);
        }
    }
}
