namespace osu.Game.Rulesets.Tau.Objects
{
    public class SliderNode
    {
        public float Time { get; set; }

        public float Angle { get; set; }

        public SliderNode(float time, float angle = 0)
        {
            Time = time;
            Angle = angle;
        }
    }
}
