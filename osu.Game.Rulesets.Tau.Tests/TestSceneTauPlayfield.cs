using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneTauPlayfield : TauTestScene
    {
        public TestSceneTauPlayfield()
        {
            Add(new TauPlayfieldAdjustmentContainer
            {
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Color4.Red,
                        Alpha = 0.2f,
                        RelativeSizeAxes = Axes.Both
                    },
                    new TauPlayfield()
                }
            });
        }
    }
}
