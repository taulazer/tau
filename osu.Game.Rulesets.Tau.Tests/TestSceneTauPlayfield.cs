using System;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Tau.Beatmaps;
using osu.Game.Rulesets.Tau.UI;
using osuTK;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneTauPlayfield : TauSkinnableTestScene
    {
        public TestSceneTauPlayfield()
        {
            TauBeatmap beatmap;

            AddStep("set beatmap", () =>
            {
                Beatmap.Value = CreateWorkingBeatmap(beatmap = new TauBeatmap());
                beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = 1000 });
                Beatmap.Value.Track.Start();
            });

            AddStep("Load playfield", () => SetContents(() => new TauPlayfield(Beatmap.Value.BeatmapInfo.BaseDifficulty)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.6f),
                FillAspectRatio = 1,
                FillMode = FillMode.Fit
            }));

            DllResourceStore dllResourceStore = new DllResourceStore(DynamicCompilationOriginal.GetType().Assembly);

            foreach (var resource in dllResourceStore.GetAvailableResources())
            {
                Console.WriteLine(resource);
            }
        }
    }
}
