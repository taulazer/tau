using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
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

        private FormCheckBox showEffects;
        private FormCheckBox showSliderEffects;
        private FormCheckBox showVisualizer;
        private FormEnumDropdown<KiaiType> kiaiType;

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (TauRulesetConfigManager)Config;

            if (config == null)
                return;

            Children = new Drawable[]
            {
                new SettingsItemV2(new FormCheckBox
                {
                    Caption = SettingStrings.HitLighting,
                    Current = config.GetBindable<bool>(TauRulesetSettings.HitLighting)
                }),
                new SettingsItemV2(showEffects = new FormCheckBox
                {
                    Caption = SettingStrings.ShowEffects,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowEffects)
                }),
                new SettingsItemV2(showSliderEffects = new FormCheckBox
                {
                    Caption = SettingStrings.ShowSliderEffects,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowSliderEffects)
                }),
                new SettingsItemV2(showVisualizer = new FormCheckBox
                {
                    Caption = SettingStrings.ShowVisualizer,
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowVisualizer)
                }),
                new SettingsItemV2(kiaiType = new FormEnumDropdown<KiaiType>()
                {
                    Caption = SettingStrings.KiaiType,
                    Current = config.GetBindable<KiaiType>(TauRulesetSettings.KiaiType)
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = SettingStrings.PlayfieldDim,
                    Current = config.GetBindable<float>(TauRulesetSettings.PlayfieldDim),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                }),
                new SettingsItemV2(new FormSliderBar<float>
                {
                    Caption = SettingStrings.NotesSize,
                    Current = config.GetBindable<float>(TauRulesetSettings.NotesSize),
                    KeyboardStep = 1f
                })
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
