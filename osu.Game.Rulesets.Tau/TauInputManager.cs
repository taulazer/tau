﻿using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau
{
    public class TauInputManager : RulesetInputManager<TauAction>
    {
        public TauInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum TauAction
    {
        LeftButton,
        RightButton,
        HardButton1,
        HardButton2,
    }
}