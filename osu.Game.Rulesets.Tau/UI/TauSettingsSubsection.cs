using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Tau.Configuration;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.UI
{
    public partial class TauSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "tau";

        public TauSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        private SettingsCheckbox showEffects;
        private SettingsCheckbox showSliderEffects;
        private SettingsCheckbox showVisualizer;
        private SettingsEnumDropdown<KiaiType> kiaiType;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (TauRulesetConfigManager)Config;

            if (config == null)
                return;

            Children = new Drawable[]
            {
                showEffects = new SettingsCheckbox
                {
                    LabelText = SettingStrings.ShowEffects,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowEffects)
                },
                showSliderEffects = new SettingsCheckbox
                {
                    LabelText = SettingStrings.ShowSliderEffects,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowSliderEffects)
                },
                showVisualizer = new SettingsCheckbox
                {
                    LabelText = SettingStrings.ShowVisualizer,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowVisualizer)
                },
                new SettingsCheckbox
                {
                    LabelText = SettingStrings.HitLighting,
                    Current = config.GetBindable<bool>(TauRulesetSettings.HitLighting)
                },
                new SettingsCheckbox
                {
                    LabelText = SettingStrings.IncreaseVisualDistinction,
                    Current = config.GetBindable<bool>(TauRulesetSettings.IncreaseVisualDistinction)
                },
                kiaiType = new SettingsEnumDropdown<KiaiType>()
                {
                    LabelText = SettingStrings.KiaiType,
                    Current = config.GetBindable<KiaiType>(TauRulesetSettings.KiaiType)
                },
                new SettingsSlider<float>
                {
                    LabelText = SettingStrings.PlayfieldDim,
                    Current = config.GetBindable<float>(TauRulesetSettings.PlayfieldDim),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<float>
                {
                    LabelText = SettingStrings.NotesSize,
                    Current = config.GetBindable<float>(TauRulesetSettings.NotesSize),
                    KeyboardStep = 1f
                },
            };

            showEffects.Current.BindValueChanged(v =>
            {
                showSliderEffects.Current.Disabled = !v.NewValue;
                showVisualizer.Current.Disabled = !v.NewValue;
                kiaiType.Current.Disabled = !v.NewValue;
            }, true);
        }
    }
}
