using osu.Game.Skinning;

namespace osu.Game.Rulesets.Tau
{
    public class TauSkinComponent : GameplaySkinComponent<TauSkinComponents>
    {
        public TauSkinComponent(TauSkinComponents component)
            : base(component)
        {
        }

        protected override string RulesetPrefix => TauRuleset.SHORT_NAME;

        protected override string ComponentName => Component.ToString().ToLower();
    }
}
