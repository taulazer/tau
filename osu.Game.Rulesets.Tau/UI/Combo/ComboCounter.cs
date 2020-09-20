using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Tau.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.UI.Combo
{
    public class ComboCounter : CircularContainer
    {
        private readonly IBeatmap beatmap;
        private readonly CircularProgress missedNotes;
        private readonly CircularProgress clearedNotes;
        private int clearedNotesCount;
        private int missedNotesCount;

        public ComboCounter(IBeatmap beatmap, bool flip = false)
        {
            this.beatmap = beatmap;

            FillMode = FillMode.Fit;
            FillAspectRatio = 1;
            RelativeSizeAxes = Axes.Both;

            Scale = new Vector2(flip ? -1 : 1, 1);

            Children = new Drawable[]
            {
                new CircularProgress
                {
                    Colour = TauPlayfield.ACCENT_COLOR.Opacity(0.25f),
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(0.2f),
                    InnerRadius = 0.16f,
                    Rotation = 55
                },
                clearedNotes = new CircularProgress
                {
                    Colour = TauPlayfield.ACCENT_COLOR,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(),
                    InnerRadius = 0.16f,
                    Rotation = 55
                },
                missedNotes = new CircularProgress
                {
                    Colour = Color4.White,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(),
                    InnerRadius = 0.16f,
                    Rotation = 55
                },
            };
        }

        private readonly Bindable<ComboSetting> setting = new Bindable<ComboSetting>(ComboSetting.Disabled);

        [BackgroundDependencyLoader(true)]
        private void load(TauRulesetConfigManager config)
        {
            config?.BindWith(TauRulesetSettings.ComboCounter, setting);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            setting.BindValueChanged(s =>
            {
                switch (s.NewValue)
                {
                    case ComboSetting.Disabled:
                        this.FadeOut(500);

                        break;

                    //TODO: Implement other modes
                    case ComboSetting.Overall:
                    case ComboSetting.Timeline:
                        this.FadeIn(500);

                        break;

                    default:
                        this.FadeOut(500);

                        break;
                }
            }, true);
        }

        public void AddResult(JudgementResult result)
        {
            if (result.Type == HitResult.None)
                return;

            var value = 0.2f / beatmap.HitObjects.Count;

            if (result.IsHit)
            {
                clearedNotes.TransformBindableTo(clearedNotes.Current, ++clearedNotesCount * value, 500, Easing.OutQuint);

                return;
            }

            missedNotes.TransformBindableTo(missedNotes.Current, ++missedNotesCount * value, 500, Easing.OutQuint);
            clearedNotes.RotateTo(missedNotesCount * value * 360 + 55, 500, Easing.OutQuint);
        }
    }
}
