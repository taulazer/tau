using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Rulesets.Tau.Difficulty;

public class TauPerformanceAttribute : PerformanceAttributes
{
    [JsonProperty("aim")]
    public double Aim { get; set; }

    [JsonProperty("speed")]
    public double Speed { get; set; }

    [JsonProperty("accuracy")]
    public double Accuracy { get; set; }

    [JsonProperty("effective_miss_count")]
    public double EffectiveMissCount { get; set; }

    public override IEnumerable<PerformanceDisplayAttribute> GetAttributesForDisplay()
    {
        foreach (var attribute in base.GetAttributesForDisplay())
        {
            yield return attribute;
        }

        yield return new PerformanceDisplayAttribute(nameof(Aim), "Aim", Aim);
        yield return new PerformanceDisplayAttribute(nameof(Accuracy), "Accuracy", Accuracy);
        yield return new PerformanceDisplayAttribute(nameof(Speed), "Speed", Speed);

    }
}
