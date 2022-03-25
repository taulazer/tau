using System.Collections.Generic;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau
{
    public class TauInputManager : RulesetInputManager<TauAction>
    {
        public IEnumerable<TauAction> PressedActions => KeyBindingContainer.PressedActions;

        public bool AllowUserPresses
        {
            set => ((TauKeyBindingContainer)KeyBindingContainer).AllowUserPresses = value;
        }

        protected override KeyBindingContainer<TauAction> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
            => new TauKeyBindingContainer(ruleset, variant, unique);

        public TauInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }

        private class TauKeyBindingContainer : RulesetKeyBindingContainer
        {
            public bool AllowUserPresses = true;

            public TauKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
                : base(ruleset, variant, unique)
            {
            }

            protected override bool Handle(UIEvent e)
                => AllowUserPresses && base.Handle(e);
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
