using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Tau
{
    public partial class TauInputManager : RulesetInputManager<TauAction>
    {
        public IEnumerable<TauAction> PressedActions => KeyBindingContainer.PressedActions;

        public bool AllowUserPresses
        {
			get => ((TauKeyBindingContainer)KeyBindingContainer).AllowUserPresses;
            set => ((TauKeyBindingContainer)KeyBindingContainer).AllowUserPresses = value;
        }

        /// <summary>
        /// Whether the user's cursor movement events should be accepted.
        /// Can be used to block only movement while still accepting button input.
        /// </summary>
        public bool AllowUserCursorMovement { get; set; } = true;

        protected override KeyBindingContainer<TauAction> CreateKeyBindingContainer(RulesetInfo ruleset, int variant, SimultaneousBindingMode unique)
            => new TauKeyBindingContainer(ruleset, variant, unique);

        public TauInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }

		[BackgroundDependencyLoader]
        private void load()
        {
            Add(new TauTouchInputMapper(this) { RelativeSizeAxes = Axes.Both });
        }

        protected override bool Handle(UIEvent e)
        {
            if ((e is MouseMoveEvent || e is TouchMoveEvent) && !AllowUserCursorMovement) return false;

            return base.Handle(e);
        }

        private partial class TauKeyBindingContainer : RulesetKeyBindingContainer
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
        [LocalisableDescription(typeof(InputStrings), nameof(InputStrings.LeftButton))]
        LeftButton,

        [LocalisableDescription(typeof(InputStrings), nameof(InputStrings.RightButton))]
        RightButton,

        [LocalisableDescription(typeof(InputStrings), nameof(InputStrings.HardButton1))]
        HardButton1,

        [LocalisableDescription(typeof(InputStrings), nameof(InputStrings.HardButton2))]
        HardButton2,
    }
}
