using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Configuration
{
    public enum KiaiType
    {
        [LocalisableDescription(typeof(KiaiTypeStrings), nameof(KiaiTypeStrings.Turbulence))]
        Turbulence,

        [LocalisableDescription(typeof(KiaiTypeStrings), nameof(KiaiTypeStrings.Classic))]
        Classic,

        [LocalisableDescription(typeof(KiaiTypeStrings), nameof(KiaiTypeStrings.None))]
        None
    }
}
