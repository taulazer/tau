using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Tau.Configuration
{
    public class TauRulesetConfigManager : RulesetConfigManager<TauRulesetSettings>
    {
        public TauRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(TauRulesetSettings.ShowEffects, true);
            SetDefault(TauRulesetSettings.ShowVisualizer, true);
            SetDefault(TauRulesetSettings.ShowSliderEffects, true);
            SetDefault(TauRulesetSettings.KiaiType, KiaiType.Turbulence);
            SetDefault(TauRulesetSettings.PlayfieldDim, 0.7f, 0, 1, 0.01f);
            SetDefault(TauRulesetSettings.NotesSize, 16f, 10, 25, 1f);
        }
    }

    public enum TauRulesetSettings
    {
        ShowEffects,
        ShowVisualizer,
        ShowSliderEffects, // There's no real reason to have a toggle for showing Kiai effects, as that's already handled under KiaiType
        KiaiType,
        PlayfieldDim,
        NotesSize,
    }
}
