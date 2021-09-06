using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
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
using osu.Game.Rulesets.Tau.Edit;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.Scoring;
using osu.Game.Rulesets.Tau.Skinning.Legacy;
using osu.Game.Rulesets.Tau.Statistics;
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
                        new MultiMod(new TauModHidden(), new TauModFadeIn()),
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
                        new TauModLite()
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
            new KeyBinding(InputKey.Space, TauAction.HardButton1),
            new KeyBinding(InputKey.LShift, TauAction.HardButton2),
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

        public override ISkin CreateLegacySkinProvider(ISkin source, IBeatmap beatmap) => new TauLegacySkinTransformer(source);

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
            private void load(TextureStore textures, FontStore store, GameHost host)
            {
                if (!textures.GetAvailableResources().Contains("Textures/tau.png"))
                    textures.AddStore(host.CreateTextureLoaderStore(ruleset.CreateResourceStore()));

                // Note to new ruleset creators:
                // This is definitely something you should try to avoid.
                // Typically resources would only be loaded inside of gameplay, NOT anywhere else.
                // Until the osu! team figures out a safe way for you to use resources out of the gameplay area (e.g mods icon),
                // Please try to avoid this at all costs.
                store.AddStore(new GlyphStore(
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
