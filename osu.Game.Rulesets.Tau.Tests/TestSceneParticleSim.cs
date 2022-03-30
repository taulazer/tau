using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Physics;
using osu.Game.Tests.Visual;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneParticleSim : OsuTestScene
    {
        List<Particle> particles = new();
        void Add (Particle particle) {
            particle.Depth = -1;
            particles.Add(particle);
            base.Add(particle);

            distanceHash.Add(particle.X, particle.Y, particle);
        }

        const float FIELD_SCALE = 50;
        DistanceHash<Particle> distanceHash = new(radius: FIELD_SCALE);

        public TestSceneParticleSim () {
            var rng = new Random(122345);

            float next (float min, float max) {
                return min + rng.NextSingle() * ( max - min );
            }

            for ( int i = 0; i < 1000; i++ ) {
                Add( new Particle {
                    Anchor = Anchor.Centre,
                    Position = new( next( -200, 200 ), next( -200, 200 ) ),
                    Velocity = new Vector2( next( -1, 1 ), next( -1, 1 ) ).Normalized() * 10
                } );
            }

            //for ( int y = 0; y < 10; y++ ) {
            //    for ( int x = -3; x <= 3; x++ ) {
            //        Add( new Particle {
            //            Anchor = Anchor.Centre,
            //            Position = new( x * 5 + 30, y * 15 + 100 ),
            //            Velocity = new Vector2( 0, -40 )
            //        } );
            //    }
            //}

            Add( new Particle {
                Anchor = Anchor.Centre,
                Mass = 100
            } );

            //for ( int y = -4; y < 4; y++ ) {
            //    for ( int x = -4; x <= 4; x++ ) {
            //        Add( new Particle {
            //            Anchor = Anchor.Centre,
            //            Position = new( x * 20, y * 20 )
            //        } );
            //    }
            //}

            // pressure
            //createScalarField( 20, 20, 18, x => x.Mass, Field.KernelBell() );
            // negative pressure gradient
            //createVectorField( 30, 30, 12, x => x.Mass, Field.Gradient(Field.KernelSpike()) );

            // viscosity
            // velocity
            createVectorField( 15, 15, 18 * 4 / 3, x => x.Velocity, Field.KernelBell() );
            // laplacian
            //createVectorField( 30, 30, 12, x => x.Velocity, Field.Laplacian( Field.KernelAsymptote()) );

            // color
            //createScalarField( 20, 20, 18, x => x.Volume, Field.KernelBell() );
            // normal into fluid
            //var normal = Field.Gradient( Field.KernelBell() );
            //createVectorField( 20, 20, 18, x => x.Volume, normal );
            // curvature
            //var lapl = Field.Laplacian( Field.KernelBell() );
            //Field.ScalarFn curvature = p => {
            //    var n = normal(p).Length;
            //    if (n == 0)
            //        return 0;
            //    else
            //        return -lapl(p)/n;
            //};
            //createScalarField( 10, 10, 36, x => x.Volume, lapl );
            //createScalarField( 30, 30, 12, x => x.Volume, curvature );
            // traction
            //Field.VectorFn traction = p => {
            //    var v = normal( p );
            //    var n = v.Length;
            //    if ( n == 0 )
            //        return Vector2.Zero;
            //    else
            //        return -lapl( p ) / n * v / n;
            //};
            //createVectorField( 30, 30, 12, x => x.Volume, traction );
            // force
            //createVectorField( 10, 10, 36, x => x.Volume, r => {
            //    var n = normal( r );
            //    if (n == Vector2.Zero)
            //        return Vector2.Zero;

            //    return -lapl( r ) * n.Normalized();
            //} );
        }

        Drawable createField<T, Tdrawable, Tdata> ( int w, int h, float res,
            Func<Vector2, T> selector, Func<Memory<T>, Tdata> analizer, Func<Vector2, Tdrawable> visualizer, Action<T, Tdrawable, Tdata> updater
        ) where Tdrawable : Drawable {
            Container<Tdrawable> container = new() {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                AutoSizeAxes = Axes.Both
            };
            Add(container);

            for ( int x = -w; x <= h; x++ ) {
                for ( int y = -h; y <= h; y++ ) {
                    container.Add( visualizer( new Vector2( x, y ) * res ) );
                }
            }

            var l = ( 2 * w + 1 ) * ( 2 * h + 1 );
            container.OnUpdate += _ => {
                using var _1 = ArrayPool<T>.Rent( l, out var data );
                int i = 0;

                foreach ( var d in container ) {
                    data[i++] = selector(d.Position);
                }

                var settings = analizer(data.AsMemory(0, l));

                i = 0;
                foreach ( var d in container ) {
                    updater( data[i++], d, settings );
                }
            };

            return container;
        }

        private static float max<T> ( Memory<T> memory, Func<T, float> selector ) {
            var max = float.NegativeInfinity;
            foreach ( var k in memory.Span ) {
                var n = selector(k);
                if ( max < n )
                    max = n;
            }
            return max;
        }
        private static float max ( Memory<float> memory ) {
            var max = float.NegativeInfinity;
            foreach ( var n in memory.Span ) {
                if ( max < n )
                    max = n;
            }
            return max;
        }
        private static float min<T> ( Memory<T> memory, Func<T, float> selector ) {
            var min = float.PositiveInfinity;
            foreach ( var k in memory.Span ) {
                var n = selector(k);
                if ( min > n )
                    min = n;
            }
            return min;
        }
        private static float min ( Memory<float> memory ) {
            var min = float.PositiveInfinity;
            foreach ( var n in memory.Span ) {
                if ( min > n )
                    min = n;
            }
            return min;
        }

        Drawable createScalarField ( int w, int h, float res, Func<Particle, float> selector, Field.ScalarFn kernel, float? from = null, float? to = null ) {
            return createField( w, h, res, pos => {
                return ParticleField<Particle>.FieldAt( pos, distanceHash.GetClose( pos, FIELD_SCALE ), selector, kernel, FIELD_SCALE );
            }, data => {
                return (from is null ? min(data) : from.Value, to is null ? max(data) : to.Value);
            }, pos => new Box {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Position = pos,
                Size = new( res )
            }, (v, d, range) => {
                d.Colour = Interpolation.ValueAt( v, Colour4.Blue, Colour4.Red, range.Item1, range.Item2 );
            } );
        }

        Drawable createVectorField ( int w, int h, float res, Func<Particle, float> selector, Field.VectorFn gradient ) {
            return createField( w, h, res, pos => {
                return ParticleField<Particle>.FieldAt( pos, distanceHash.GetClose( pos, FIELD_SCALE ), selector, gradient, FIELD_SCALE );
            }, data => {
                return (min(data, x => x.Length), max(data, x => x.Length));
            }, pos => new Box {
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.Centre,
                Position = pos
            }, (v, d, range) => {
                d.Size = range.Item1 == range.Item2 ? Vector2.Zero : new( res * ( v.Length - range.Item1 ) / (range.Item2 - range.Item1) / 2, res / 8 );
                d.Rotation = MathF.Atan2( v.Y, v.X ) / MathF.PI * 180;
                d.Colour = Interpolation.ValueAt( v.Length, Colour4.Blue, Colour4.Green, range.Item1, range.Item2 );
            } );
        }

        Drawable createVectorField ( int w, int h, float res, Func<Particle, Vector2> selector, Field.ScalarFn kernel ) {
            return createField( w, h, res, pos => {
                return ParticleField<Particle>.FieldAt( pos, distanceHash.GetClose( pos, FIELD_SCALE ), selector, kernel, FIELD_SCALE );
            }, data => {
                return (min( data, x => x.Length ), max( data, x => x.Length ));
            }, pos => new Box {
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.Centre,
                Position = pos
            }, ( v, d, range ) => {
                d.Size = range.Item1 == range.Item2 ? Vector2.Zero : new( res * ( v.Length - range.Item1 ) / ( range.Item2 - range.Item1 ) / 2, res / 8 );
                d.Rotation = MathF.Atan2( v.Y, v.X ) / MathF.PI * 180;
                d.Colour = Interpolation.ValueAt( v.Length, Colour4.Blue, Colour4.Green, range.Item1, range.Item2 );
            } );
        }

        protected override void Update () {
            base.Update();

            Vector2 pressureGradientField ( Particle i ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, distanceHash.GetClose( i.Position, FIELD_SCALE ),
                    j => j.Mass * ( j.Density + i.Density ) / 2 / j.Density,
                    Field.Gradient( Field.KernelSpike() ),
                    FIELD_SCALE
                );
            }

            float densityField ( Particle i ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, distanceHash.GetClose( i.Position, FIELD_SCALE ),
                    j => j.Mass,
                    Field.KernelSpike(),
                    FIELD_SCALE
                );
            }

            Vector2 viscosityField ( Particle i ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, distanceHash.GetClose( i.Position, FIELD_SCALE ),
                    j => j.Mass * ( j.Velocity - i.Velocity ) / j.Density,
                    Field.KernelBell(),
                    FIELD_SCALE
                );
            }

            Vector2 colorGradient ( Particle i ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, distanceHash.GetClose( i.Position, FIELD_SCALE ),
                    j => j.Volume,
                    Field.Gradient( Field.KernelBell() ),
                    FIELD_SCALE
                );
            }
            float colorLaplacian ( Particle i ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, distanceHash.GetClose( i.Position, FIELD_SCALE ),
                    j => j.Volume,
                    Field.Laplacian( Field.KernelBell() ),
                    FIELD_SCALE
                );
            }
            Vector2 sufraceTensionForce ( Particle i ) {
                var n = colorGradient( i );
                if ( n == Vector2.Zero )
                    return Vector2.Zero;

                return -colorLaplacian( i ) * n.Normalized();
            }

            Vector2 forceField ( Particle p ) {
                return -pressureGradientField( p ) + viscosityField( p ) /*+ sufraceTensionForce( p ) / 6*/;
            }

            Vector2 accelerationField ( Particle p ) {
                // acceleration is force / mass
                return forceField( p ) / densityField( p );
            }

            var delta = MathF.Min( (float)Time.Elapsed, 100 ) / 1000 * 14;
            using ( ArrayPool<Vector2>.Rent( particles.Count, out var acceleration ) ) {
                int i = 0;
                foreach ( var p in particles ) {
                    acceleration[ i++ ] = accelerationField( p );
                }

                i = 0;
                foreach ( var p in particles ) {
                    p.Velocity += acceleration[ i++ ] * delta;
                    p.Position += p.Velocity * delta;

                    if ( p.X < -360 ) {
                        p.X = -360;
                        p.Velocity.X *= -1;
                    }
                    if ( p.X > 360 ) {
                        p.X = 360;
                        p.Velocity.X *= -1;
                    }
                    if ( p.Y < -360 ) {
                        p.Y = -360;
                        p.Velocity.Y *= -1;
                    }
                    if ( p.Y > 360 ) {
                        p.Y = 360;
                        p.Velocity.Y *= -1;
                    }
                }
            }

            distanceHash.Clear();
            foreach (var i in particles) {
                distanceHash.Add(i.X, i.Y, i);
            }
        }

        private class Particle : Circle, IHasPosition {
            public Vector2 Velocity;

            private float mass = 1;
            private float density = 1;
            public float Mass {
                get => mass;
                set {
                    mass = value;
                    Size = new( MathF.Sqrt( mass / density ) * 4 );
                }
            }
            public float Density {
                get => density;
                set {
                    density = value;
                    Size = new( MathF.Sqrt( mass / density ) * 4 );
                }
            }
            public float Volume => mass / density;

            public Particle () {
                Origin = Anchor.Centre;
                Mass = 1;
            }
        }
    }

    public static class ArrayPool<T> {
        public static IDisposable Rent (int length, out T[] arr) {
            arr = System.Buffers.ArrayPool<T>.Shared.Rent( length );
            return new ReturnArray(arr);
        }

        private struct ReturnArray : IDisposable {
            T[] arr;

            public ReturnArray ( T[] arr ) {
                this.arr = arr;
            }

            public void Dispose () {
                System.Buffers.ArrayPool<T>.Shared.Return( arr );
            }
        }
    }

    public class DistanceHash<T> {
        Stack<List<T>> listPool = new();
        private Dictionary<(int x, int y), List<T>> hash = new();
        public readonly float Radius;

        public DistanceHash ( float radius ) {
            Radius = radius;
        }

        private List<T> nextList () {
            return listPool.TryPop( out var l ) ? l : new();
        }
        public void Clear () {
            foreach (var l in hash.Values) {
                l.Clear();
                listPool.Push(l);
            }
            hash.Clear();
        }

        public void Add ( Vector2 pos, T value ) {
            Add( pos.X, pos.Y, value );
        }
        public void Add (float x, float y, T value) {
            int x2 = (int)MathF.Round( x / Radius );
            int y2 = (int)MathF.Round( y / Radius );

            if (!hash.TryGetValue((x2, y2), out var list))
                hash.Add((x2, y2), list = nextList());

            list.Add(value);
        }

        public IEnumerable<T> GetClose ( Vector2 pos, float distance ) {
            return GetClose( pos.X, pos.Y, distance );
        }
        public IEnumerable<T> GetClose ( float x, float y, float distance ) {
            int xFrom = (int)MathF.Round( ( x - distance ) / Radius );
            int xTo = (int)MathF.Round( ( x + distance ) / Radius );
            int yFrom = (int)MathF.Round( ( y - distance ) / Radius );
            int yTo = (int)MathF.Round( ( y + distance ) / Radius );

            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    if ( hash.TryGetValue((i, j), out var list) ) {
                        foreach ( var k in list ) {
                            yield return k;
                        }
                    }
                }
            }
        }
    }
}
