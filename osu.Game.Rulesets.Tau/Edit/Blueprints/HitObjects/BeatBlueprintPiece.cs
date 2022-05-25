using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Rulesets.Tau.Objects;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Edit.Blueprints.HitObjects;

public class BeatBlueprintPiece : BlueprintPiece<Beat>
{
    public BindableFloat NoteSize = new(16f);

    public BeatBlueprintPiece()
    {
        Origin = Anchor.Centre;
        Size = new Vector2(NoteSize.Default);
        Anchor = Anchor.Centre;
        RelativePositionAxes = Axes.Both;

        InternalChild = new Container
        {
            Masking = true,
            BorderThickness = 10,
            BorderColour = Color4.Yellow,
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Box
                {
                    AlwaysPresent = true,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both
                }
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load(OsuColour colours)
    {
        Colour = colours.Yellow;
    }

    public void UpdateFrom(Beat hitObject)
    {
        base.UpdateFrom(hitObject);
    }
}
