using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests.Mods
{
    public class TestSceneRoundabout : OsuManualInputManagerTestScene
    {
        private TauModRoundabout roundabout = null!;
        private TauCursor cursor = null!;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = base.CreateChildDependencies(parent);
            var modDependencies = new DependencyContainer(dependencies);

            modDependencies.CacheAs(typeof(IReadOnlyList<Mod>), new Mod[] { roundabout = new TauModRoundabout() });

            return modDependencies;
        }

        [Test]
        public void TestLockedClockwise()
        {
            AddStep("set direction to CW", () => roundabout.Direction.Value = RotationDirection.Clockwise);
            AddStep("set to 0°", () => moveMouse(0));
            AddStep("add cursor", () =>
            {
                Clear();
                Add(cursor = new TauCursor());
            });
            AddStep("rotate CW 45°", () => moveMouse(-45));
            AddAssert("cursor rotation is correct", () => cursor.DrawablePaddle.Rotation == 45);
            AddStep("rotate CC 45°", () => moveMouse(45));
            AddAssert("cursor rotation is correct", () => cursor.DrawablePaddle.Rotation != 315);
        }

        [Test]
        public void TestLockedCounterClockwise()
        {
            AddStep("set direction to CC", () => roundabout.Direction.Value = RotationDirection.Counterclockwise);
            AddStep("set to 0°", () => moveMouse(0));
            AddStep("add cursor", () =>
            {
                Clear();
                Add(cursor = new TauCursor());
            });
            AddStep("rotate CW 45°", () => moveMouse(-45));
            AddAssert("cursor rotation is correct", () => cursor.DrawablePaddle.Rotation != 45);
            AddStep("rotate CC 45°", () => moveMouse(45));
            AddAssert("cursor rotation is correct", () => cursor.DrawablePaddle.Rotation == 315);
        }

        private void moveMouse(float angle)
        {
            InputManager.MoveMouseTo(Content.ScreenSpaceDrawQuad.Centre + Extensions.FromPolarCoordinates(200, -angle));
        }
    }
}
