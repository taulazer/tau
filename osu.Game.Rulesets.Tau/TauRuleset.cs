using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.Statistics;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;

namespace osu.Game.Rulesets.Tau
{
    public class TauRuleset : Ruleset
    {
        public const string SHORT_NAME = "tau";

        public override string Description => SHORT_NAME;
        public override string ShortName => SHORT_NAME;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new TauDrawableRuleset(this, beatmap, mods);
        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TauBeatmapConverter(this, beatmap);
        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new TauDifficultyCalculator(RulesetInfo, beatmap);
        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new TauRulesetConfigManager(settings, RulesetInfo);
        public override RulesetSettingsSubsection CreateSettings() => new TauSettingsSubsection(this);
        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new TauReplayFrame();
        public override ScoreProcessor CreateScoreProcessor() => new TauScoreProcessor(this);

        public override IEnumerable<Mod> GetModsFor(ModType type)
            => type switch
            {
                ModType.DifficultyReduction => new Mod[]
                {
                    new TauModEasy(),
                    new TauModNoFail(),
                    new MultiMod(new TauModHalfTime(), new TauModDaycore())
                },
                ModType.DifficultyIncrease => new Mod[]
                {
                    new TauModHardRock(),
                    new MultiMod(new TauModSuddenDeath(), new TauModPerfect()),
                    new MultiMod(new TauModDoubleTime(), new TauModNightcore()),
                    new MultiMod(new TauModFadeOut(), new TauModFadeIn()),
                    new TauModFlashlight(),
                    new TauModInverse()
                },
                ModType.Automation => new Mod[]
                {
                    new MultiMod(new TauModAutoplay(), new TauModCinema()),
                    new TauModRelax(),
                    new TauModAutopilot()
                },
                ModType.Conversion => new Mod[]
                {
                    new TauModDifficultyAdjust(),
                    new TauModLite()
                },
                ModType.Fun => new Mod[]
                {
                    new MultiMod(new ModWindUp(), new ModWindDown()),
                    new ModAdaptiveSpeed(),
                    new TauModImpossibleSliders()
                },
                _ => Enumerable.Empty<Mod>()
            };

        protected override IEnumerable<HitResult> GetValidHitResults()
        {
            return new[]
            {
                HitResult.Great,
                HitResult.Ok,
                HitResult.Miss,
            };
        }

        public override StatisticRow[] CreateStatisticsForScore(ScoreInfo score, IBeatmap playableBeatmap) => new[]
        {
            new StatisticRow
            {
                Columns = new[]
                {
                    new StatisticItem("Timing Distribution", new HitEventTimingDistributionGraph(score.HitEvents)
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 250
                    }),
                }
            },
            new StatisticRow
            {
                Columns = new[]
                {
                    new StatisticItem("Paddle Distribution", new PaddleDistributionGraph(score.HitEvents, playableBeatmap)
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 250,
                    })
                }
            },
            new StatisticRow
            {
                Columns = new[]
                {
                    new StatisticItem(String.Empty, new SimpleStatisticTable(3, new SimpleStatisticItem[]
                    {
                        new UnstableRate(score.HitEvents)
                    }))
                }
            }
        };

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, TauAction.LeftButton),
            new KeyBinding(InputKey.X, TauAction.RightButton),
            new KeyBinding(InputKey.MouseLeft, TauAction.LeftButton),
            new KeyBinding(InputKey.MouseRight, TauAction.RightButton),
            new KeyBinding(InputKey.Space, TauAction.HardButton1),
            new KeyBinding(InputKey.LShift, TauAction.HardButton2),
        };
    }
}
