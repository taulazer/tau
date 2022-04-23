using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Tau.Judgements;
using osu.Game.Rulesets.Tau.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Tau.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Objects.Drawables
{
    public class DrawableBeat : DrawableAngledTauHitObject<Beat>
    {
        public Drawable DrawableBox;

        public DrawableBeat()
            : this(null)
        {
        }

        public DrawableBeat(Beat hitObject)
            : base(hitObject)
        {
            Name = "Beat track";
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;
            Size = Vector2.One;

            AddInternal(DrawableBox = new Container
            {
                RelativePositionAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                AlwaysPresent = true,
                Size = new Vector2(NoteSize.Default),
                Child = new BeatPiece()
            });

            angleBindable.BindValueChanged(r => Rotation = r.NewValue);
        }

        private readonly BindableFloat angleBindable = new();

        protected override void OnApply()
        {
            base.OnApply();
            angleBindable.BindTo(HitObject.AngleBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();
            angleBindable.UnbindFrom(HitObject.AngleBindable);
        }

        [Resolved(canBeNull: true)]
        private TauCachedProperties properties { get; set; }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            DrawableBox.FadeIn(HitObject.TimeFadeIn);

            if (properties != null && properties.InverseModEnabled.Value)
                DrawableBox.MoveToY(-1.0f);

            DrawableBox.MoveToY(-0.5f, HitObject.TimePreempt);
        }

        [BackgroundDependencyLoader()]
        private void load()
        {
            NoteSize.BindValueChanged(value => DrawableBox.Size = new Vector2(value.NewValue), true);
        }

        protected override JudgementResult CreateResult(Judgement judgement) => new TauJudgementResult(HitObject, judgement);

        [Resolved]
        private OsuColour colour { get; set; }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            const double time_fade_hit = 250, time_fade_miss = 400;
            var offset = new Vector2(0, -.1f);

            if (properties != null && properties.InverseModEnabled.Value)
                offset.Y = -offset.Y;

            switch (state)
            {
                case ArmedState.Hit:
                    DrawableBox.ScaleTo(2f, time_fade_hit, Easing.OutQuint)
                               .FadeColour(colour.ForHitResult(Result.Type), time_fade_hit, Easing.OutQuint)
                               .MoveToOffset(offset, time_fade_hit, Easing.OutQuint)
                               .FadeOut(time_fade_hit);

                    this.Delay(time_fade_hit).Expire();

                    break;

                case ArmedState.Miss:
                    DrawableBox.ScaleTo(0.5f, time_fade_miss, Easing.InQuint)
                               .FadeColour(Color4.Red, time_fade_miss, Easing.OutQuint)
                               .MoveToOffset(offset, time_fade_miss, Easing.OutQuint)
                               .FadeOut(time_fade_miss);

                    this.Delay(time_fade_miss).Expire();

                    break;
            }
        }
    }
}
