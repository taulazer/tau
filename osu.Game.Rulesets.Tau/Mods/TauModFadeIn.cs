﻿using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Tau.Localisation;

namespace osu.Game.Rulesets.Tau.Mods
{
    public class TauModFadeIn : TauModHidden
    {
        public override IconUsage? Icon => TauIcons.ModFadeIn;

        public override string Acronym => "FI";

        // Modification from osu!mania's description of Hidden mod.
        public override LocalisableString Description => ModStrings.FadeInDescription;
        protected override MaskingMode Mode => MaskingMode.FadeIn;
        protected override float InitialCoverage => 0.25f;
    }
}
