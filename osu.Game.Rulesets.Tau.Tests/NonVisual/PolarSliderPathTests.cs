using NUnit.Framework;
using NUnit.Framework.Legacy;
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

            ClassicAssert.AreEqual(50, path.AngleAt(-50));
            ClassicAssert.AreEqual(50, path.AngleAt(0));
            ClassicAssert.AreEqual(55, path.AngleAt(50));
            ClassicAssert.AreEqual(60, path.AngleAt(100));
            ClassicAssert.AreEqual(65, path.AngleAt(150));
            ClassicAssert.AreEqual(70, path.AngleAt(200));
            ClassicAssert.AreEqual(65, path.AngleAt(250));
            ClassicAssert.AreEqual(60, path.AngleAt(300));
            ClassicAssert.AreEqual(55, path.AngleAt(350));
            ClassicAssert.AreEqual(50, path.AngleAt(400));
            ClassicAssert.AreEqual(50, path.AngleAt(450));
        }

        [Test]
        public void TestNodeAtNegatives()
        {
            var path = new PolarSliderPath(new SliderNode[]
            {
                new(0, -10),
                new(200, 10),
            });

            ClassicAssert.AreEqual(-10, path.AngleAt(-50));
            ClassicAssert.AreEqual(-10, path.AngleAt(0));
            ClassicAssert.AreEqual(-5, path.AngleAt(50));
            ClassicAssert.AreEqual(0, path.AngleAt(100));
            ClassicAssert.AreEqual(5, path.AngleAt(150));
            ClassicAssert.AreEqual(10, path.AngleAt(200));
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

            ClassicAssert.IsNotEmpty(nodes);
            ClassicAssert.AreEqual(2, nodes.Length);
            ClassicAssert.AreEqual(60, nodes[0].Angle);
            ClassicAssert.AreEqual(70, nodes[1].Angle);

            nodes = path.NodesBetween(50, 60).ToArray();
            ClassicAssert.IsEmpty(nodes);

            nodes = path.NodesBetween(90, 110).ToArray();
            ClassicAssert.IsNotEmpty(nodes);
            ClassicAssert.AreEqual(1, nodes.Length);
            ClassicAssert.AreEqual(60, nodes[0].Angle);
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

            ClassicAssert.AreEqual(0, path.Version.Value);
            ClassicAssert.AreEqual(40, path.CalculatedDistance);
            ClassicAssert.AreEqual(0, path.Version.Value);
            ClassicAssert.AreEqual(20, path.CalculateLazyDistance(10));
            ClassicAssert.AreEqual(0, path.CalculateLazyDistance(20));
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
            ClassicAssert.AreEqual(2, segments.Length);
            ClassicAssert.AreEqual(60, segments[0].From.Angle);
            ClassicAssert.AreEqual(60, segments[1].To.Angle);

            segments = path.SegmentsBetween(50, 100).ToArray();
            ClassicAssert.AreEqual(1, segments.Length);
            ClassicAssert.AreEqual(55, segments[0].From.Angle);
            ClassicAssert.AreEqual(60, segments[0].To.Angle);

            segments = path.SegmentsBetween(0, 400).ToArray();
            ClassicAssert.AreEqual(2, segments.Length);
            ClassicAssert.AreEqual(50, segments[0].From.Angle);
            ClassicAssert.AreEqual(70, segments[0].To.Angle);
            ClassicAssert.AreEqual(50, segments[1].To.Angle);
        }
    }
}
