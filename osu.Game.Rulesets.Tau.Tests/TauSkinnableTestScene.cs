using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public abstract class TauSkinnableTestScene : SkinnableTestScene
    {
        private Container content;

        protected override Container<Drawable> Content
        {
            get
            {
                if (content == null)
                    base.Content.Add(content = new TauInputManager(new TauRuleset().RulesetInfo));

                return content;
            }
        }

        protected override Ruleset CreateRulesetForSkinProvider() => new TauRuleset();
    }
}
