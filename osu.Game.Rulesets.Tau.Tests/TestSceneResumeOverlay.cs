using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Rulesets.Tau.UI;
using osu.Game.Screens.Play;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Tau.Tests
{
    public partial class TestSceneResumeOverlay : OsuManualInputManagerTestScene
    {
        [Cached]
        private TauCachedProperties properties { get; set; } = new();

        public TestSceneResumeOverlay()
        {
            ManualTauInputManager tauInputManager;
            CursorContainer cursor;
            ResumeOverlay resume;

            bool resumeFired = false;

            Child = tauInputManager = new ManualTauInputManager(new TauRuleset().RulesetInfo)
            {
                Children = new Drawable[]
                {
                    cursor = new CursorContainer(),
                    resume = new TauResumeOverlay()
                    {
                        GameplayCursor = cursor
                    }
                }
            };

            resume.ResumeAction = () => resumeFired = true;

            AddStep("move mouse to center", () => InputManager.MoveMouseTo(ScreenSpaceDrawQuad.TopLeft));
            AddStep("show", () => resume.Show());

            AddStep("move mouse away", () => InputManager.MoveMouseTo(ScreenSpaceDrawQuad.BottomRight));
            AddStep("click", () => tauInputManager.GameClick());
            AddAssert("not dismissed", () => !resumeFired && resume.State.Value == Visibility.Visible);

            AddStep("move mouse back", () => InputManager.MoveMouseTo(ScreenSpaceDrawQuad.TopLeft));
            AddStep("click", () => tauInputManager.GameClick());
            AddAssert("dismissed", () => resumeFired && resume.State.Value == Visibility.Hidden);
        }

        private partial class ManualTauInputManager : TauInputManager
        {
            public ManualTauInputManager(RulesetInfo ruleset)
                : base(ruleset)
            {
            }

            public void GameClick()
            {
                KeyBindingContainer.TriggerPressed(TauAction.LeftButton);
                KeyBindingContainer.TriggerReleased(TauAction.LeftButton);
            }
        }
    }
}
