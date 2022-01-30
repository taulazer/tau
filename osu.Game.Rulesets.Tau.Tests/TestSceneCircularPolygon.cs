using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using osu.Game.Rulesets.Tau.Graphics.Primitives;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneCircularPolygon : TestScene
    {
        public TestSceneCircularPolygon()
        {
            var drawable = new CircleDrawable
            {
                Size = new Vector2(200),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            Add(drawable);
            AddSliderStep("Resolution", 3, 100, 10, v => drawable.Resolution = v);
            AddSliderStep("Size X", 10, 300, 200, v => drawable.Size = new Vector2(v, drawable.Size.Y));
            AddSliderStep("Size Y", 10, 300, 200, v => drawable.Size = new Vector2(drawable.Size.X, v));
        }

        public class CircleDrawable : Sprite
        {
            public CircleDrawable()
            {
                Texture = Texture.WhitePixel;
            }

            public int Resolution = 10;

            public override RectangleF BoundingBox => toCircle(ToParentSpace(LayoutRectangle)).AABBFloat;

            private Circle toCircle(Quad q) => new(Resolution, q);

            public override bool Contains(Vector2 screenSpacePos) => toCircle(ScreenSpaceDrawQuad).Contains(screenSpacePos);

            protected override bool OnHover(HoverEvent e)
            {
                Colour = Color4.Red;
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);
                Colour = Color4.White;
            }

            protected override DrawNode CreateDrawNode() => new CircleDrawNode(this);

            private class CircleDrawNode : SpriteDrawNode
            {
                public new CircleDrawable Source => (CircleDrawable)base.Source;

                public CircleDrawNode(Sprite source)
                    : base(source)
                {
                }

                protected override void Blit(Action<TexturedVertex2D> vertexAction)
                {
                    var polygon = Source.toCircle(ScreenSpaceDrawQuad);

                    DrawClipped(ref polygon, Texture, DrawColourInfo.Colour, null, null,
                        new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height), TextureCoords);
                }

                protected override void BlitOpaqueInterior(Action<TexturedVertex2D> vertexAction)
                {
                    var polygon = Source.toCircle(ConservativeScreenSpaceDrawQuad);

                    DrawClipped(ref polygon, Texture, DrawColourInfo.Colour, vertexAction: vertexAction);
                }
            }
        }
    }
}
