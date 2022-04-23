using osu.Framework.Bindables;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.Tau.Mods {
    public class TauModNoScope : ModNoScope, IUpdatableByPlayfield {
        public override string Description => "Where's the cursor?";

        [SettingSource(
            "Hidden at combo",
            "The combo count at which the cursor becomes completely hidden",
            SettingControlType = typeof( SettingsSlider<int, HiddenComboSlider> )
        )]
        public override BindableInt HiddenComboCount { get; } = new BindableInt {
            Default = 10,
            Value = 10,
            MinValue = 0,
            MaxValue = 50,
        };

        public void Update ( Playfield playfield ) {
            bool shouldAlwaysShowCursor = IsBreakTime.Value;
            float targetAlpha = shouldAlwaysShowCursor ? 1 : ComboBasedAlpha;
            playfield.Cursor.Alpha = (float)Interpolation.Lerp( playfield.Cursor.Alpha, targetAlpha, Math.Clamp( playfield.Time.Elapsed / TRANSITION_DURATION, 0, 1 ) );
        }
    }
}
