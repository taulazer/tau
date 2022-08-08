using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Tau.Configuration;

namespace osu.Game.Rulesets.Tau.UI
{
    public class TauSettingsSubsection : RulesetSettingsSubsection
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
                    LabelText = "Show Effects",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowEffects)
                },
                showSliderEffects = new SettingsCheckbox
                {
                    LabelText = "Show Slider Effects",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowSliderEffects)
                },
                showVisualizer = new SettingsCheckbox
                {
                    LabelText = "Show Visualizer",
                    Current = config.GetBindable<bool>(TauRulesetSettings.ShowVisualizer)
                },
                new SettingsCheckbox
                {
                    LabelText = "Hit Lighting",
                    Current = config.GetBindable<bool>(TauRulesetSettings.HitLighting)
                },
                kiaiType = new SettingsEnumDropdown<KiaiType>()
                {
                    LabelText = "Kiai Type",
                    Current = config.GetBindable<KiaiType>(TauRulesetSettings.KiaiType)
                },
                new SettingsSlider<float>
                {
                    LabelText = "Playfield Dim",
                    Current = config.GetBindable<float>(TauRulesetSettings.PlayfieldDim),
                    KeyboardStep = 0.01f,
                    DisplayAsPercentage = true
                },
                new SettingsSlider<float>
                {
                    LabelText = "Notes Size",
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
