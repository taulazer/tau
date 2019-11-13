// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Input.Bindings;
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
        [Description("Left tick button")]
        LeftButton,

        [Description("Right tick button")]
        RightButton,

        [Description("Hard beat button")]
        HardButton
    }
}
