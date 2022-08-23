using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableStrictHardBeat : DrawableAngledTauHitObject<StrictHardBeat>
    {
        protected override TauAction[] Actions { get; } =
        {
            TauAction.HardButton1,
            TauAction.HardButton2
        };

        private readonly StrictHardBeatPiece piece;

        public DrawableStrictHardBeat()
            : this(null)
        {
        }

        public DrawableStrictHardBeat(StrictHardBeat obj)
            : base(obj)
        {
            Name = "Strict Hard beat track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Alpha = 0f;
            AlwaysPresent = true;

            AddInternal(piece = new StrictHardBeatPiece
            {
                RelativeSizeAxes = Axes.Both,
                NoteSize = { BindTarget = NoteSize },
            });
        }

        protected override void OnApply()
        {
            base.OnApply();
            piece.AngleRange.Value = HitObject.Range;
        }

        protected override bool CheckForValidation()
        {
            if (CheckValidation == null)
                return false;

            var firstResult = CheckValidation((HitObject.Angle + (float)(HitObject.Range / 2) + (float)HitObject.Range).Normalize());
            var secondResult = CheckValidation((HitObject.Angle + (float)(HitObject.Range / 2)).Normalize());

            return firstResult.IsValid && secondResult.IsValid;
        }

        protected override float GetCurrentOffset()
            => (float)(HitObject.Range / 2);

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeIn(HitObject.TimeFadeIn);

            if (properties != null && properties.InverseModEnabled.Value)
                this.ResizeTo(2);

            piece.ResizeTo(Vector2.One, HitObject.TimePreempt);
        }

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;
            float scaleHit = 1.25f, scaleMiss = 1.1f;

            if (properties != null && properties.InverseModEnabled.Value)
            {
                scaleHit = 0.75f;
                scaleMiss = 0.9f;
            }

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;

                    break;

                case ArmedState.Hit:
                    this.ScaleTo(scaleHit, time_fade_hit, Easing.OutQuint)
                        .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_hit);

                    break;

                case ArmedState.Miss:
                    this.FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .ResizeTo(scaleMiss, time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_miss);

                    break;
            }
        }
    }
}
