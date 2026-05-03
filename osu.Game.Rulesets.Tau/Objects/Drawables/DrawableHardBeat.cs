using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public partial class DrawableHardBeat : DrawableTauHitObject<HardBeat>
    {
        public TauAction? HitAction { get; private set; }

        private readonly Container<DrawableHardBeatNestedHitObject> nestedHitObjectsContainer;

        public DrawableHardBeat()
            : this(null)
        {
        }

        public DrawableHardBeat(HardBeat obj)
            : base(obj)
        {
            Name = "Hard beat track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.Zero;
            Alpha = 0f;
            AlwaysPresent = true;

            AddRangeInternal([
                new HardBeatPiece
                {
                    RelativeSizeAxes = Axes.Both,
                    NoteSize = { BindTarget = NoteSize }
                },
                nestedHitObjectsContainer = new Container<DrawableHardBeatNestedHitObject>()
            ]);
        }

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            this.FadeIn(HitObject.TimeFadeIn);
            this.ResizeTo(1, HitObject.TimePreempt);
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableHardBeatNestedHitObject hb:
                    nestedHitObjectsContainer.Add(hb);
                    return;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            nestedHitObjectsContainer.Clear(false);
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case HardBeatNestedHitObject nested:
                    return new DrawableHardBeatNestedHitObject(nested);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        public override bool OnPressed(KeyBindingPressEvent<TauAction> e)
        {
            bool isPressed = base.OnPressed(e);

            if (isPressed)
                HitAction = e.Action;

            return isPressed;
        }

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;
            const float scale_hit = 1.25f, scale_miss = 1.1f;

            switch (state)
            {
                case ArmedState.Idle:
                    LifetimeStart = HitObject.StartTime - HitObject.TimePreempt;

                    break;

                case ArmedState.Hit:
                    this.ScaleTo(scale_hit, time_fade_hit, Easing.OutQuint)
                        .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_hit);

                    break;

                case ArmedState.Miss:
                    this.FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                        .ResizeTo(scale_miss, time_fade_hit, Easing.OutQuint)
                        .FadeOut(time_fade_miss);

                    break;
            }
        }
    }
}
