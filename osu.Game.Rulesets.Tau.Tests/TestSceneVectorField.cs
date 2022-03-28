using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Testing;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneVectorField : TestScene
    {
        private Container container;

        public TestSceneVectorField()
        {
            Add(container = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(500),
                Children = new[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.CornflowerBlue.Opacity(0.5f)
                    }
                }
            });

            addLines();
        }

        private void addLines()
        {
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    var fromCenterX = x - 25;
                    var fromCenterY = y - 25;

                    addLine(new Vector2(x * 10, y * 10), new Vector2(10));
                }
            }
        }

        private void addLine(Vector2 position, Vector2 vector)
        {
            var pos = Vector2.Divide(position, 500);
            container.Add(new Path
            {
                RelativePositionAxes = Axes.Both,
                Position = pos,
                PathRadius = 1,
                Colour = ColourInfo.GradientHorizontal(Color4.Red, Color4.Blue),
                Vertices = new[]
                {
                    Vector2.Zero,
                    vector
                }
            });
        }
    }
}
