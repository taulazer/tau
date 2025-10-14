﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Objects;
using osu.Game.Rulesets.Tau.Objects.Drawables;
using osu.Game.Rulesets.Tau.UI;

namespace osu.Game.Rulesets.Tau.Tests.Objects
{
    [TestFixture]
    public partial class TestSceneSliderHardBeat : TauTestScene
    {
        private int depthIndex;

        private TauPlayfieldAdjustmentContainer container;
        private BindableBool increaseVisualDistinction = new BindableBool();

        [BackgroundDependencyLoader]
        private void load() {
            var config = (TauRulesetConfigManager)RulesetConfigs.GetConfigFor(Ruleset.Value.CreateInstance()).AsNonNull();
            config.BindWith(TauRulesetSettings.IncreaseVisualDistinction, increaseVisualDistinction);
        }

        [Test]
        public void TestSingleSlider()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));
            AddStep("visual distinction disabled", () => increaseVisualDistinction.Value = false);

            AddStep("Miss Single", () => container.Add(testSingle()));
            AddStep("Hit Single", () => container.Add(testSingle(true)));
            AddUntilStep("Wait for object despawn", () => !container.Any(h => h is DrawableSlider { AllJudged: false }));
        }

        [Test]
        public void TestVisuallyDistinctSingleSlider()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));
            AddStep("visual distinction enabled", () => increaseVisualDistinction.Value = true);

            AddStep("Miss single", () => container.Add(testSingle()));
            AddStep("Hit single", () => container.Add(testSingle(true)));
            AddUntilStep("Wait for object despawn", () => !container.Any(h => h is DrawableSlider { AllJudged: false }));
        }

        [Test]
        public void TestSliderPerformance()
        {
            AddStep("clear screen", Clear);
            AddStep("add container", () => Add(container = new TauPlayfieldAdjustmentContainer()));

            AddStep("Miss many", () => container.AddRange(testMultiple(100)));
            AddStep("Hit many", () => container.AddRange(testMultiple(100, true)));
        }

        private IEnumerable<Drawable> testMultiple(int count, bool auto = false)
            => Enumerable.Range(0, count).Select(x => testSingle(auto, 1000 + x * 100));

        private Drawable testSingle(bool auto = false, double timeOffset = 1000)
        {
            var slider = createSlider(timeOffset);

            slider.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new TestDrawableSlider(slider, auto)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Depth = depthIndex++
            };
        }

        private Slider createSlider(double timeOffset)
            => new()
            {
                StartTime = Time.Current + timeOffset,
                IsHard = true,
                Path = new PolarSliderPath(new SliderNode[]
                {
                    new(0, 0),
                    new(500, 90),
                    new(1000, 180),
                })
            };

        private partial class TestDrawableSlider : DrawableSlider
        {
            private readonly bool auto;

            public TestDrawableSlider(Slider h, bool auto)
                : base(h)
            {
                this.auto = auto;
            }

            protected override void CheckForResult(bool userTriggered, double timeOffset)
            {
                if (auto && !userTriggered && timeOffset > 0)
                {
                    // force success
                    ApplyResult(HitResult.Great);
                }
                else
                    base.CheckForResult(userTriggered, timeOffset);
            }
        }
    }
}
