using System;
using System.Collections.Generic;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Difficulty;
using osu.Game.Rulesets.Tau.Mods;
using osu.Game.Rulesets.Tau.Replays;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau
{
    public class TauRuleset : Ruleset
    {
        public const string SHORT_NAME = "tau";

        public override string Description => SHORT_NAME;
        public override string ShortName => SHORT_NAME;

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.Automation:
                    return new Mod[]
                    {
                        new TauModAutoplay()
                    };

                default:
                    return ArraySegment<Mod>.Empty;
            }
        }

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            => new TauDrawableRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            => new TauBeatmapConverter(this, beatmap);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            => new TauDifficultyCalculator(RulesetInfo, beatmap);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings)
            => new TauRulesetConfigManager(settings, RulesetInfo);

        public override RulesetSettingsSubsection CreateSettings()
            => new TauSettingsSubsection(this);

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new TauReplayFrame();

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, TauAction.LeftButton),
            new KeyBinding(InputKey.X, TauAction.RightButton),
            new KeyBinding(InputKey.Space, TauAction.HardButton1),
            new KeyBinding(InputKey.LShift, TauAction.HardButton2),
        };
    }
}
