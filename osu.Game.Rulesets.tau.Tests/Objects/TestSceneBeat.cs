using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public class TestSceneBeat : OsuTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(DrawabletauBeatObject)
        };


        private readonly Container content;
        protected override Container<Drawable> Content => content;
        private int depthIndex = 0;

        public TestSceneBeat()
        {
            base.Content.Add(content = new TauInputManager(new RulesetInfo { ID = 0 }));

            AddStep("Miss Single", () => testSingle());
            AddStep("Hit Single", () => testSingle(true));
        }

        private void testSingle(bool auto = false)
        {
            var beatObject = new TauBeatObject
            {
                StartTime = Time.Current + 1000,
                PositionToEnd = new Vector2(0, 0),
                Angle = 0,
            };

            beatObject.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty { });

            var drawable = CreateDrawableBeat(beatObject, auto);

            Add(drawable);
        }

        protected virtual TestDrawableBeat CreateDrawableBeat(TauBeatObject beat, bool auto) => new TestDrawableBeat(beat, auto)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Depth = depthIndex++,
        };

        protected class TestDrawableBeat : DrawabletauBeatObject
        {
            private readonly bool auto;

            public TestDrawableBeat(TauBeatObject h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            public void TriggerJudgement() => UpdateResult(true);

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                {
                    // force success
                    ApplyResult(r => r.Type = HitResult.Perfect);
                }
                else
                    base.CheckForResult(userTriggered, timeOffset);
            }
        }
    }
}
