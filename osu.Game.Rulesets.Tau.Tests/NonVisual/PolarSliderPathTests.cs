using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Tau.Objects;

namespace osu.Game.Rulesets.Tau.Tests.NonVisual
{
    public class PolarSliderPathTests
    {
        [Test]
        public void TestNodeAt()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, 50),
                new(200, 70),
                new(400, 50),
            });

            Assert.AreEqual(50, path.NodeAt(-50).Angle);
            Assert.AreEqual(50, path.NodeAt(0).Angle);
            Assert.AreEqual(55, path.NodeAt(50).Angle);
            Assert.AreEqual(60, path.NodeAt(100).Angle);
            Assert.AreEqual(65, path.NodeAt(150).Angle);
            Assert.AreEqual(70, path.NodeAt(200).Angle);
            Assert.AreEqual(65, path.NodeAt(250).Angle);
            Assert.AreEqual(60, path.NodeAt(300).Angle);
            Assert.AreEqual(55, path.NodeAt(350).Angle);
            Assert.AreEqual(50, path.NodeAt(400).Angle);
            Assert.AreEqual(50, path.NodeAt(450).Angle);
        }

        [Test]
        public void TestNodeAtNegatives()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, -10),
                new(200, 10),
            });

            Assert.AreEqual(-10, path.NodeAt(-50).Angle);
            Assert.AreEqual(-10, path.NodeAt(0).Angle);
            Assert.AreEqual(-5, path.NodeAt(50).Angle);
            Assert.AreEqual(0, path.NodeAt(100).Angle);
            Assert.AreEqual(5, path.NodeAt(150).Angle);
            Assert.AreEqual(10, path.NodeAt(200).Angle);
        }

        [Test]
        public void TestNodesBetween()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, 50),
                new(100, 60),
                new(200, 70),
            });

            var nodes = path.NodesBetween(50, 250).ToArray();

            Assert.IsNotEmpty(nodes);
            Assert.AreEqual(2, nodes.Length);
            Assert.AreEqual(60, nodes[0].Angle);
            Assert.AreEqual(70, nodes[1].Angle);
        }

        [Test]
        public void TestCalculatedDistance()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, 50),
                new(200, 70),
                new(400, 50),
            });

            Assert.AreEqual(1, path.Version.Value);
            Assert.AreEqual(40, path.CalculatedDistance);
            Assert.AreEqual(1, path.Version.Value);
        }
    }
}
