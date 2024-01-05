using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Platform;
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
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.Statistics;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;
using osuTK;

namespace osu.Game.Rulesets.Tau
{
    public partial class TauRuleset : Ruleset
    {
        public const string SHORT_NAME = "tau";

        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;

        public override string Description => SHORT_NAME;
        public override string ShortName => SHORT_NAME;
        public override string PlayingVerb => "Slicing beats";

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new TauRulesetConfigManager(settings, RulesetInfo);
        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TauBeatmapConverter(this, beatmap);
        public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new BeatmapProcessor(beatmap);
        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new TauDrawableRuleset(this, beatmap, mods);
        public override ScoreProcessor CreateScoreProcessor() => new TauScoreProcessor(this);
        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new TauReplayFrame();
        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new TauDifficultyCalculator(RulesetInfo, beatmap);
        public override PerformanceCalculator CreatePerformanceCalculator() => new TauPerformanceCalculator();
        public override RulesetSettingsSubsection CreateSettings() => new TauSettingsSubsection(this);

        public override Drawable CreateIcon() => new TauIcon(this);

        public override IEnumerable<Mod> GetModsFor(ModType type)
            => type switch
            {
                ModType.DifficultyReduction => new Mod[]
                {
                    new TauModEasy(),
                    new TauModNoFail(),
                    new MultiMod(new TauModHalfTime(), new TauModDaycore()),
                    new TauModLenience()
                },
                ModType.DifficultyIncrease => new Mod[]
                {
                    new TauModHardRock(),
                    new MultiMod(new TauModSuddenDeath(), new TauModPerfect()),
                    new MultiMod(new TauModDoubleTime(), new TauModNightcore()),
                    new MultiMod(new TauModFadeOut(), new TauModFadeIn()),
                    new TauModFlashlight(),
                    new TauModStrict()
                },
                ModType.Automation => new Mod[]
                {
                    new MultiMod(new TauModAutoplay(), new TauModShowoffAutoplay(), new TauModCinema()),
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
                    new TauModInverse(),
                    new TauModImpossibleSliders(),
                    new TauModRoundabout(),
                    new TauModNoScope(),
                    new TauModTraceable(),
                    new TauModDual()
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

                HitResult.SmallTickHit,
                HitResult.SmallTickMiss
            };
        }

        public override LocalisableString GetDisplayNameForHitResult(HitResult result)
            => result switch
            {
                HitResult.SmallTickHit => UiStrings.Ticks,
                _ => base.GetDisplayNameForHitResult(result)
            };

        public override StatisticItem[] CreateStatisticsForScore(ScoreInfo score, IBeatmap playableBeatmap) => new[]
        {
            new StatisticItem(UiStrings.TimingDistribution, () => new HitEventTimingDistributionGraph(score.HitEvents)
            {
                RelativeSizeAxes = Axes.X,
                Height = 250
            }, true),

            new StatisticItem(UiStrings.PaddleDistribution, () => new PaddleDistributionGraph(score.HitEvents, playableBeatmap)
            {
                RelativeSizeAxes = Axes.X,
                Height = 250
            }, true),

            new StatisticItem(string.Empty, () => new SimpleStatisticTable(3, new SimpleStatisticItem[]
            {
                new AverageHitError(score.HitEvents),
                new UnstableRate(score.HitEvents)
            }), true)
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

        private partial class TauIcon : CompositeDrawable
        {
            private readonly Ruleset ruleset;

            public TauIcon(Ruleset ruleset)
            {
                this.ruleset = ruleset;
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(FontStore store, GameHost host)
            {
                // NOTE: To new ruleset developers, please do not ever ever EVER do this.
                //       Typically rulesets resources should be created inside of gameplay, NOT anywhere else.
                //       Until the osu! team figures out a safe way for you to use resources out of the gameplay area (e.g mods icon),
                //       Please try to avoid this at all costs.
                //
                //       ~ Nora
                store.AddTextureSource(new GlyphStore(
                    new ResourceStore<byte[]>(ruleset.CreateResourceStore()),
                    "Fonts/tauFont",
                    host.CreateTextureLoaderStore(ruleset.CreateResourceStore())));

                AddRangeInternal(new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Regular.Circle,
                    },
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = TauIcons.Tau,
                        Scale = new Vector2(0.9f)
                    },
                });
            }
        }
    }
}
