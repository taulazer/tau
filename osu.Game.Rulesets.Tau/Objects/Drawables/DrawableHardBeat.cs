using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Skinning.Default;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableHardBeat : DrawableTauHitObject, IKeyBindingHandler<TauAction>
    {
        protected override TauAction[] HitActions { get; set; } = new[]
        {
            TauAction.HardButton1,
            TauAction.HardButton2
        };

        public SkinnableDrawable Circle;
        public float HitScale = 1.25f;
        public float MissScale = 1.1f;

        public DrawableHardBeat()
            : this(null)
        {
        }

        public DrawableHardBeat(TauHitObject hitObject)
            : base(hitObject)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.Zero;
            Alpha = 0f;

            AddInternal(Circle = new SkinnableDrawable(new TauSkinComponent(TauSkinComponents.HardBeat), _ => new HardBeatPiece(), ConfineMode.ScaleToFit));

            Position = Vector2.Zero;
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeIn(HitObject.TimeFadeIn);
            this.ResizeTo(1, HitObject.TimePreempt);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None || CheckHittable?.Invoke(this, Time.Current) == false)
                return;

            ApplyResult(r => r.Type = result);
        }

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;
                    HitAction = null;

                    break;

                case ArmedState.Hit:
                    this.ScaleTo(HitScale, time_fade_hit, Easing.OutQuint)
                        .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_hit);

                    break;

                case ArmedState.Miss:
                    this.FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .ResizeTo(MissScale, time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_miss);

                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<TauAction> e)
        {
            if (AllJudged)
                return false;

            if (HitActions.Contains(e.Action))
                return UpdateResult(true);

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<TauAction> e)
        {
        }
    }
}
