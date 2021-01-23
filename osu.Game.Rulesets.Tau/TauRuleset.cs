using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Edit;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.Skinning.Legacy;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Tau
{
    public class TauRuleset : Ruleset
    {
        public const string SHORT_NAME = "tau";
        public override string Description => SHORT_NAME;

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableTauRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new TauBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) =>
            new TauDifficultyCalculator(this, beatmap);

        // public override PerformanceCalculator CreatePerformanceCalculator(WorkingBeatmap beatmap, ScoreInfo score) =>
        //     new TauPerformanceCalculator(this, beatmap, score);
        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new TauModEasy(),
                        new TauModNoFail(),
                        new MultiMod(new TauModHalfTime(), new TauModDaycore()),
                        new TauModAutoHold(),
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new TauModHardRock(),
                        new TauModSuddenDeath(),
                        new MultiMod(new TauModDoubleTime(), new TauModNightcore()),
                        new TauModHidden(),
                        new MultiMod(new TauModFlashlight(), new TauModBlinds()),
                    };

                case ModType.Automation:
                    return new Mod[]
                    {
                        new MultiMod(new TauModAutoplay(), new TauModCinema()),
                        new TauModRelax(),
                    };

                case ModType.Conversion:
                    return new Mod[]
                    {
                        new TauModDifficultyAdjust(),
                    };

                case ModType.Fun:
                    return new Mod[]
                    {
                        new MultiMod(new ModWindUp(), new ModWindDown()),
                    };

                default:
                    return new Mod[] { null };
            }
        }

        public override string ShortName => SHORT_NAME;

        public override string PlayingVerb => "Hitting beats";

        public override RulesetSettingsSubsection CreateSettings() => new TauSettingsSubsection(this);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new TauRulesetConfigManager(settings, RulesetInfo);

        public override ScoreProcessor CreateScoreProcessor() => new TauScoreProcessor();

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, TauAction.LeftButton),
            new KeyBinding(InputKey.X, TauAction.RightButton),
            new KeyBinding(InputKey.MouseLeft, TauAction.LeftButton),
            new KeyBinding(InputKey.MouseRight, TauAction.RightButton),
            new KeyBinding(InputKey.Space, TauAction.HardButton),
        };

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
                    new StatisticItem(string.Empty, new SimpleStatisticTable(3, new SimpleStatisticItem[]
                    {
                        new UnstableRate(score.HitEvents)
                    }))
                }
            }
        };

        public override Drawable CreateIcon() => new TauIcon(this);

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new TauReplayFrame();

        public override HitObjectComposer CreateHitObjectComposer() => new TauHitObjectComposer(this);

        public override IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => new BeatmapProcessor(beatmap);

        public override ISkin CreateLegacySkinProvider(ISkinSource source, IBeatmap beatmap) => new TauLegacySkinTransformer(source);

        protected override IEnumerable<HitResult> GetValidHitResults()
        {
            return new[]
            {
                HitResult.Great,
                HitResult.Ok,
                HitResult.Miss,
            };
        }

        private class TauIcon : CompositeDrawable
        {
            private readonly Ruleset ruleset;
            public TauIcon(Ruleset ruleset)
            {
                this.ruleset = ruleset;
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore textures, GameHost host)
            {
                if (!textures.GetAvailableResources().Contains("Textures/tau.png"))
                    textures.AddStore(host.CreateTextureLoaderStore(ruleset.CreateResourceStore()));

                AddRangeInternal(new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Regular.Circle,
                    },
                    new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(1),
                        Scale = new Vector2(.625f),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = textures.Get("Textures/tau")
                    }
                });
            }
        }
    }
}
