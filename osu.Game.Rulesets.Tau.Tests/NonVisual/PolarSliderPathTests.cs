using NUnit.Framework;
using osu.Game.Rulesets.Tau.Objects;
using System.Linq;

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

            Assert.AreEqual(50, path.AngleAt(-50));
            Assert.AreEqual(50, path.AngleAt(0));
            Assert.AreEqual(55, path.AngleAt(50));
            Assert.AreEqual(60, path.AngleAt(100));
            Assert.AreEqual(65, path.AngleAt(150));
            Assert.AreEqual(70, path.AngleAt(200));
            Assert.AreEqual(65, path.AngleAt(250));
            Assert.AreEqual(60, path.AngleAt(300));
            Assert.AreEqual(55, path.AngleAt(350));
            Assert.AreEqual(50, path.AngleAt(400));
            Assert.AreEqual(50, path.AngleAt(450));
        }

        [Test]
        public void TestNodeAtNegatives()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, -10),
                new(200, 10),
            });

            Assert.AreEqual(-10, path.AngleAt(-50));
            Assert.AreEqual(-10, path.AngleAt(0));
            Assert.AreEqual(-5, path.AngleAt(50));
            Assert.AreEqual(0, path.AngleAt(100));
            Assert.AreEqual(5, path.AngleAt(150));
            Assert.AreEqual(10, path.AngleAt(200));
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

            nodes = path.NodesBetween(50, 60).ToArray();
            Assert.IsEmpty(nodes);

            nodes = path.NodesBetween(90, 110).ToArray();
            Assert.IsNotEmpty(nodes);
            Assert.AreEqual(1, nodes.Length);
            Assert.AreEqual(60, nodes[0].Angle);
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

            Assert.AreEqual(0, path.Version.Value);
            Assert.AreEqual(40, path.CalculatedDistance);
            Assert.AreEqual(0, path.Version.Value);
            Assert.AreEqual(20, path.CalculateLazyDistance(10));
            Assert.AreEqual(0, path.CalculateLazyDistance(20));
        }

        [Test]
        public void TestSegments()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, 50),
                new(200, 70),
                new(400, 50),
            });

            var segments = path.SegmentsBetween(100, 300).ToArray();
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual(60, segments[0].From.Angle);
            Assert.AreEqual(60, segments[1].To.Angle);

            segments = path.SegmentsBetween(50, 100).ToArray();
            Assert.AreEqual(1, segments.Length);
            Assert.AreEqual(55, segments[0].From.Angle);
            Assert.AreEqual(60, segments[0].To.Angle);

            segments = path.SegmentsBetween(0, 400).ToArray();
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual(50, segments[0].From.Angle);
            Assert.AreEqual(70, segments[0].To.Angle);
            Assert.AreEqual(50, segments[1].To.Angle);
        }
    }
}
