using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestScenePlayfieldMask : OsuTestScene
    {
        protected override Ruleset CreateRuleset() => new TauRuleset();

        public TestScenePlayfieldMask()
        {
            Add(new PlayfieldMaskDrawable()
            {
                RelativeSizeAxes = Axes.Both,
            });
        }
    }
}
