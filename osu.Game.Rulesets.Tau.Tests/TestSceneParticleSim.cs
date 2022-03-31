#define DRAW_PARTICLES

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Tau.Physics;
using osu.Game.Tests.Visual;
using osuTK;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace osu.Game.Rulesets.Tau.Tests
{
    public class TestSceneParticleSim : OsuTestScene
    {
        List<Particle> particles = new();
        List<ParticleData> particleData = new();
        void Add (Particle particle) {
            particle.Depth = -1;
            var data = new ParticleData {
                Mass = particle.Mass,
                Density = particle.Density,
                Volume = particle.Volume,
                Position = particle.Position,
                Velocity = particle.Velocity
            };
            particles.Add(particle);
            particleData.Add(data);
#if DRAW_PARTICLES
            base.Add(particle);
#endif

            distanceHash.Add(particle.X, particle.Y, data );
        }

        const float FIELD_SCALE = 50;
        ArrayDistanceHash<ParticleData> distanceHash = new(radius: FIELD_SCALE, -360, 360, -360, 360);

        public TestSceneParticleSim () {
            var rng = new Random(122345);

            Add(cursor = new Box {
                Size = new(10),
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Alpha = 0
            } );

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
#if DRAW_PARTICLES
            //createScalarField( 15, 15, 24, x => x.Velocity, Field.KernelBell(), fn => Field.Curl(fn) );
            createVectorField( 15, 15, 24, x => x.Velocity, Field.KernelBell() );
#endif
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

        Drawable createScalarField ( int w, int h, float res, Func<ParticleData, float> selector, Field.ScalarFn kernel, float? from = null, float? to = null ) {
            return createField( w, h, res, pos => {
                using ( distanceHash.GetClose( pos, FIELD_SCALE, particles.Count, out var arr ) )
                    return ParticleField<ParticleData>.FieldAt( pos, arr, selector, kernel, FIELD_SCALE );
            }, data => {
                return (from is null ? min( data ) : from.Value, to is null ? max( data ) : to.Value);
            }, pos => new Box {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Position = pos,
                Size = new( res )
            }, ( v, d, range ) => {
                d.Colour = Interpolation.ValueAt( v, Colour4.Blue, Colour4.Red, range.Item1, range.Item2 );
            } );
        }

        Drawable createScalarField ( int w, int h, float res, Func<ParticleData, Vector2> selector, Field.ScalarFn kernel, Func<Field.VectorFn, Field.ScalarFn> transformer, float? from = null, float? to = null ) {
            return createField( w, h, res, pos => {
                using ( distanceHash.GetClose( pos, FIELD_SCALE, particles.Count, out var arr ) )
                    return ParticleField<ParticleData>.FieldAt( pos, arr, selector, kernel, transformer, FIELD_SCALE );
            }, data => {
                return (from is null ? min( data ) : from.Value, to is null ? max( data ) : to.Value);
            }, pos => new Box {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Position = pos,
                Size = new( res )
            }, ( v, d, range ) => {
                d.Colour = Interpolation.ValueAt( v, Colour4.Blue, Colour4.Red, range.Item1, range.Item2 );
            } );
        }

        Drawable createVectorField ( int w, int h, float res, Func<ParticleData, float> selector, Field.VectorFn gradient ) {
            return createField( w, h, res, pos => {
                using ( distanceHash.GetClose( pos, FIELD_SCALE, particles.Count, out var arr ) )
                    return ParticleField<ParticleData>.FieldAt( pos, arr, selector, gradient, FIELD_SCALE );
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

        Drawable createVectorField ( int w, int h, float res, Func<ParticleData, Vector2> selector, Field.ScalarFn kernel ) {
            return createField( w, h, res, pos => {
                using ( distanceHash.GetClose( pos, FIELD_SCALE, particles.Count, out var arr ) )
                    return ParticleField<ParticleData>.FieldAt( pos, arr, selector, kernel, FIELD_SCALE );
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

        Vector2 lastMousePos = Vector2.Zero;
        Vector2 mousePos = Vector2.Zero;
        Box cursor;
        protected override bool OnMouseMove ( MouseMoveEvent e ) {
            if ( !e.CurrentState.Mouse.Buttons.HasAnyButtonPressed )
                return false;

            var prev = cursor.Parent.ToLocalSpace(e.ScreenSpaceLastMousePosition) - cursor.Parent.DrawSize / 2;
            var next = cursor.Parent.ToLocalSpace(e.ScreenSpaceMousePosition) - cursor.Parent.DrawSize / 2;

            if (prev != lastMousePos) {
                lastMousePos = next;
                mousePos = next;
            }
            else {
                lastMousePos = mousePos;
                mousePos = next;
            }

            cursor.Position = mousePos;

            return false;
        }

        static Field.ScalarFn kernelSpike = Field.KernelSpike();
        static Field.ScalarFn kernelBell = Field.KernelBell();
        static Field.VectorFn spikeGradient = Field.KernelSpikeGradient();
        static Field.VectorFn bellGradient = Field.Gradient(kernelBell);
        static Field.ScalarFn bellLaplacian = Field.Laplacian(kernelBell);
        protected override void Update () {
            base.Update();

            var delta = MathF.Min( (float)Time.Elapsed, 100 ) / 1000 * 14;
            if (lastMousePos != mousePos) {
                var drag = (mousePos - lastMousePos) / delta;
                var factor = 1 - MathF.Pow( 1 - 0.9f, delta ); // 0 -> 0, 1 -> 0.9, 2 -> 0.99

                distanceHash.ForeachClose( mousePos, FIELD_SCALE, (ref ParticleData i) => {
                    i.Velocity = i.Velocity * (1 - factor) + drag * factor;
                } );

                lastMousePos = mousePos;
            }

            Vector2 pressureGradientField ( Particle i, Span<Particle> neighborhood ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, neighborhood,
                    j => j.Mass * ( j.Density + i.Density ) / (2 * j.Density),
                    spikeGradient,
                    FIELD_SCALE
                );
            }

            float densityField ( Particle i, Span<Particle> neighborhood ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, neighborhood,
                    j => j.Mass,
                    kernelSpike,
                    FIELD_SCALE
                );
            }

            Vector2 viscosityField ( Particle i, Span<Particle> neighborhood ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, neighborhood,
                    j => j.Mass * ( j.Velocity - i.Velocity ) / j.Density,
                    kernelBell,
                    FIELD_SCALE
                );
            }

            Vector2 colorGradient ( Particle i, Span<Particle> neighborhood ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, neighborhood,
                    j => j.Volume,
                    bellGradient,
                    FIELD_SCALE
                );
            }
            float colorLaplacian ( Particle i, Span<Particle> neighborhood ) {
                return ParticleField<Particle>.FieldAt(
                    i.Position, neighborhood,
                    j => j.Volume,
                    bellLaplacian,
                    FIELD_SCALE
                );
            }
            Vector2 sufraceTensionForce ( Particle i, Span<Particle> neighborhood ) {
                var n = colorGradient( i, neighborhood );
                if ( n == Vector2.Zero )
                    return Vector2.Zero;

                return -colorLaplacian( i, neighborhood ) * n.Normalized();
            }

            Vector2 forceField ( Particle p, Span<Particle> neighborhood ) {
                return -pressureGradientField( p, neighborhood ) + viscosityField( p, neighborhood ) /*+ sufraceTensionForce( p ) / 6*/;
            }

            Vector2 accelerationField ( Particle p, Span<Particle> neighborhood ) {
                // acceleration is force / mass
                return forceField( p, neighborhood ) / densityField( p, neighborhood );
            }

            const float h = 1; // this is a parameter. you can modify it
            const float scaleInv = 1 / FIELD_SCALE;
            const float h2 = h * h;
            const float h6 = h2 * h2 * h2;
            const float spikeGradientFactor = -(-45f / MathF.PI / h6) / 2;
            const float kernelBellFactor = 315f / 64 / MathF.PI / h6;
            const float kernelSpikeFactor = 15f / MathF.PI / h6;
            Vector2 temp;
            Vector2 optimizedAccelerationField ( Particle p ) {
                var ownPosition = p.Position;
                var ownVelocity = p.Velocity;
                var ownDensity = p.Density;

                Vector2 pressureGradient = Vector2.Zero;
                Vector2 viscosity = Vector2.Zero;
                float density = 0;

                var x = ownPosition.X - distanceHash.minX;
                var y = ownPosition.Y - distanceHash.minY;
                int xFrom = (int)Math.Clamp( MathF.Floor( ( x - FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxX );
                int xTo = (int)Math.Clamp( MathF.Floor( ( x + FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxX );
                int yFrom = (int)Math.Clamp( MathF.Floor( ( y - FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxY );
                int yTo = (int)Math.Clamp( MathF.Floor( ( y + FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxY );
                var hash = distanceHash.Hash;

                // inlining distanceHash method
                for ( int i = xFrom; i <= xTo; i++ ) {
                    for ( int j = yFrom; j <= yTo; j++ ) {
                        var list = hash[ i, j ];
                        // merging ParticleField<T>.FieldAt
                        for ( int k = 0; k < list.Count; k++ ) {
                            var particle = list[k];
                            // merging kernels
                            // using Vector2 ref methods, we dont copy vector2s each time
                            Vector2.Subtract( ref ownPosition, ref particle.Position, out temp );
                            Vector2.Multiply( ref temp, scaleInv, out temp );

                            var r2 = temp.LengthSquared;
                            if ( r2 >= h2 )
                                continue;

                            var rInv = MathHelper.InverseSqrtFast( r2 );
                            var r = 1f / rInv;
                            if ( r <= 0 )
                                continue;

                            var volume = particle.Volume;
                            var hmr = h - r;
                            var h2mr2 = hmr * ( h + r ); // (a-b)(a+b) = a^2 - b^2

                            Vector2.Multiply( ref temp, spikeGradientFactor * ( h2 * rInv - 2 * h + r ) * volume * ( particle.Density + ownDensity ), out temp );
                            Vector2.Add( ref pressureGradient, ref temp, out pressureGradient );
                            Vector2.Subtract( ref particle.Velocity, ref ownVelocity, out temp );
                            Vector2.Multiply( ref temp, volume * kernelBellFactor * h2mr2 * h2mr2 * h2mr2, out temp );
                            Vector2.Add( ref viscosity, ref temp, out viscosity );
                            density += particle.Mass * kernelSpikeFactor * hmr * hmr * hmr;
                        }
                    }
                }

                var force = pressureGradient + viscosity /*+ sufraceTensionForce( p ) / 6*/;
                return force / density;
            }

            using ( ArrayPool<Vector2>.Rent( particles.Count, out var acceleration ) ) {
                int n = 0;

                //foreach ( var p in particles ) {
                //    acceleration[ n++ ] = optimizedAccelerationField( p );
                //    //using ( distanceHash.GetClose( p.Position, FIELD_SCALE, particles.Count, out var neighborghood ) ) {
                //    //    acceleration[ n++ ] = accelerationField( p, neighborhood );
                //    //}
                //}

                var hash = distanceHash.Hash;
                var particleList = CollectionsMarshal.AsSpan(particles);
                for ( int l = 0; l < particleList.Length; l++ ) {
                    var p = particleList[l];
                    // inlining optimizedAccelerationField
                    var ownPosition = p.Position;
                    var ownVelocity = p.Velocity;
                    var ownDensity = p.Density;

                    Vector2 pressureGradient = Vector2.Zero;
                    Vector2 viscosity = Vector2.Zero;
                    float density = 0;

                    var x = ownPosition.X - distanceHash.minX;
                    var y = ownPosition.Y - distanceHash.minY;
                    int xFrom = (int)Math.Clamp( MathF.Floor( ( x - FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxX );
                    int xTo = (int)Math.Clamp( MathF.Floor( ( x + FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxX );
                    int yFrom = (int)Math.Clamp( MathF.Floor( ( y - FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxY );
                    int yTo = (int)Math.Clamp( MathF.Floor( ( y + FIELD_SCALE ) * distanceHash.RadiusInverse ), 0, distanceHash.maxY );

                    // inlining distanceHash method
                    for ( int i = xFrom; i <= xTo; i++ ) {
                        for ( int j = yFrom; j <= yTo; j++ ) {
                            var list = hash[ i, j ];
                            // merging ParticleField<T>.FieldAt
                            for ( int k = 0; k < list.Count; k++ ) {
                                var particle = list[ k ];
                                // merging kernels
                                // using Vector2 ref methods, we dont copy vector2s each time
                                Vector2.Subtract( ref ownPosition, ref particle.Position, out temp );
                                Vector2.Multiply( ref temp, scaleInv, out temp );

                                var r2 = temp.LengthSquared;
                                if ( r2 >= h2 )
                                    continue;

                                var rInv = MathHelper.InverseSqrtFast( r2 );
                                var r = 1f / rInv;
                                if ( r <= 0 )
                                    continue;

                                var volume = particle.Volume;
                                var hmr = h - r;
                                var h2mr2 = hmr * ( h + r ); // (a-b)(a+b) = a^2 - b^2

                                Vector2.Multiply( ref temp, spikeGradientFactor * ( h2 * rInv - 2 * h + r ) * volume * ( particle.Density + ownDensity ), out temp );
                                Vector2.Add( ref pressureGradient, ref temp, out pressureGradient );
                                Vector2.Subtract( ref particle.Velocity, ref ownVelocity, out temp );
                                Vector2.Multiply( ref temp, volume * kernelBellFactor * h2mr2 * h2mr2 * h2mr2, out temp );
                                Vector2.Add( ref viscosity, ref temp, out viscosity );
                                density += particle.Mass * kernelSpikeFactor * hmr * hmr * hmr;
                            }
                        }
                    }

                    Vector2.Add(ref pressureGradient, ref viscosity, out temp);
                    Vector2.Divide(ref temp, density, out acceleration[n++]);
                }

                n = 0;
                foreach ( var p in particles ) {
                    Vector2.Multiply(ref acceleration[n++], delta, out temp);
                    Vector2.Add(ref p.Velocity, ref temp, out p.Velocity);
                    Vector2.Multiply(ref p.Velocity, delta, out temp);
                    p.Position += temp;

                    if ( p.X < -360 ) {
                        p.X = -360;
                        p.Velocity.X *= -1;
                    }
                    if ( p.X < -340 ) {
                        p.Velocity.X += delta;
                    }
                    if ( p.X > 360 ) {
                        p.X = 360;
                        p.Velocity.X *= -1;
                    }
                    if ( p.X > 340 ) {
                        p.Velocity.X -= delta;
                    }
                    if ( p.Y < -360 ) {
                        p.Y = -360;
                        p.Velocity.Y *= -1;
                    }
                    if ( p.Y < -340 ) {
                        p.Velocity.Y += delta;
                    }
                    if ( p.Y > 360 ) {
                        p.Y = 360;
                        p.Velocity.Y *= -1;
                    }
                    if ( p.Y > 340 ) {
                        p.Velocity.Y -= delta;
                    }
                }
            }

            distanceHash.Clear();
            foreach (var i in particles) {
                distanceHash.Add(i.X, i.Y, new() {
                    Mass = i.Mass,
                    Density = i.Density,
                    Volume = i.Volume,
                    Position = i.Position,
                    Velocity = i.Velocity
                } );
            }
        }

        private struct ParticleData : IHasPosition {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Mass;
            public float Density;
            public float Volume;

            Vector2 IHasPosition.Position => Position;
            public float X => Position.X;
            public float Y => Position.Y;
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
                    if ( hash.TryGetValue( (i, j), out var list ) ) {
                        for ( int k = 0; k < list.Count; k++ ) {
                            yield return list[ k ];
                        }
                    }
                }
            }
        }

        public IDisposable GetClose ( Vector2 pos, float distance, out Span<T> arr ) {
            return GetClose( pos.X, pos.Y, distance, out arr );
        }
        public IDisposable GetClose ( float x, float y, float distance, out Span<T> arr ) {
            int xFrom = (int)MathF.Round( ( x - distance ) / Radius );
            int xTo = (int)MathF.Round( ( x + distance ) / Radius );
            int yFrom = (int)MathF.Round( ( y - distance ) / Radius );
            int yTo = (int)MathF.Round( ( y + distance ) / Radius );

            using ( ArrayPool<List<T>>.Rent( (xTo - xFrom + 1) * (yTo - yTo + 1), out var lists ) ) {
                int count = 0;
                int listCount = 0;

                for ( int i = xFrom; i <= xTo; i++ ) {
                    for ( int j = yFrom; j <= yTo; j++ ) {
                        if ( hash.TryGetValue( (i, j), out var list ) ) {
                            count += list.Count;
                            lists[ listCount++ ] = list;
                        }
                    }
                }

                var lease = ArrayPool<T>.Rent( count, out var array );

                count = 0;
                for ( int i = 0; i < listCount; i++ ) {
                    var list = lists[ i ];
                    for ( int j = 0; j < list.Count; j++ ) {
                        array[count++] = list[j];
                    }
                }

                arr = array.AsSpan(0, count);
                return lease;
            }
        }
    }

    public class ArrayDistanceHash<T> {
        private List<T>[,] hash;
        public List<T>[,] Hash => hash;
        public readonly float Radius;
        public readonly float RadiusInverse;
        public readonly float minX;
        public readonly float minY;
        public readonly int maxX;
        public readonly int maxY;

        public ArrayDistanceHash ( float radius, float minX, float maxX, float minY, float maxY ) {
            Radius = radius;
            RadiusInverse = 1 / radius;
            this.minX = minX;
            this.minY = minY;
            this.maxX = (int)Math.Ceiling( ( maxX - minX ) / radius ) - 1;
            this.maxY = (int)Math.Ceiling( ( maxY - minY ) / radius ) - 1;
            hash = new List<T>[this.maxX + 1,this.maxY + 1];
            for ( int x = 0; x <= this.maxX; x++ ) {
                for ( int y = 0; y <= this.maxY; y++ ) {
                    hash[x, y] = new();
                }
            }
        }

        public void Clear () {
            foreach ( var l in hash ) {
                l.Clear();
            }
        }

        public void Add ( Vector2 pos, T value ) {
            Add( pos.X, pos.Y, value );
        }
        public void Add ( float x, float y, T value ) {
            int x2 = (int)MathF.Floor( (x - minX) * RadiusInverse );
            int y2 = (int)MathF.Floor( (y - minY) * RadiusInverse );

            hash[x2, y2].Add( value );
        }

        public IEnumerable<T> GetClose ( Vector2 pos, float distance ) {
            return GetClose( pos.X, pos.Y, distance );
        }
        public IEnumerable<T> GetClose ( float x, float y, float distance ) {
            x -= minX;
            y -= minY;
            int xFrom = (int)Math.Clamp( MathF.Floor( ( x - distance ) * RadiusInverse ), 0, maxX );
            int xTo = (int)Math.Clamp( MathF.Floor( ( x + distance ) * RadiusInverse ), 0, maxX );
            int yFrom = (int)Math.Clamp( MathF.Floor( ( y - distance ) * RadiusInverse ), 0, maxY );
            int yTo = (int)Math.Clamp( MathF.Floor( ( y + distance ) * RadiusInverse ), 0, maxY );

            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[i, j];
                    for ( int k = 0; k < list.Count; k++ ) {
                        yield return list[ k ];
                    }
                }
            }
        }

        public IDisposable GetClose ( Vector2 pos, float distance, out Span<T> arr ) {
            return GetClose( pos.X, pos.Y, distance, out arr );
        }
        public IDisposable GetClose ( float x, float y, float distance, out Span<T> arr ) {
            x -= minX;
            y -= minY;
            int xFrom = (int)Math.Clamp( MathF.Floor( ( x - distance ) * RadiusInverse ), 0, maxX );
            int xTo = (int)Math.Clamp( MathF.Floor( ( x + distance ) * RadiusInverse ), 0, maxX );
            int yFrom = (int)Math.Clamp( MathF.Floor( ( y - distance ) * RadiusInverse ), 0, maxY );
            int yTo = (int)Math.Clamp( MathF.Floor( ( y + distance ) * RadiusInverse ), 0, maxY );

            int count = 0;
            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[ i, j ];
                    count += list.Count;
                }
            }

            var lease = ArrayPool<T>.Rent( count, out var array );

            count = 0;
            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[ i, j ];
                    for ( int k = 0; k < list.Count; k++ ) {
                        array[ count++ ] = list[ k ];
                    }
                }
            }

            arr = array.AsSpan( 0, count );
            return lease;
        }

        public IDisposable GetClose ( Vector2 pos, float distance, int size, out Span<T> arr ) {
            return GetClose( pos.X, pos.Y, distance, size, out arr );
        }
        public IDisposable GetClose ( float x, float y, float distance, int size, out Span<T> arr ) {
            x -= minX;
            y -= minY;
            int xFrom = (int)Math.Clamp( MathF.Floor( ( x - distance ) * RadiusInverse ), 0, maxX );
            int xTo = (int)Math.Clamp( MathF.Floor( ( x + distance ) * RadiusInverse ), 0, maxX );
            int yFrom = (int)Math.Clamp( MathF.Floor( ( y - distance ) * RadiusInverse ), 0, maxY );
            int yTo = (int)Math.Clamp( MathF.Floor( ( y + distance ) * RadiusInverse ), 0, maxY );

            var lease = ArrayPool<T>.Rent( size, out var array );

            int count = 0;
            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[ i, j ];
                    for ( int k = 0; k < list.Count; k++ ) {
                        array[ count++ ] = list[ k ];
                    }
                }
            }

            arr = array.AsSpan( 0, count );
            return lease;
        }

        public delegate void ForeachAction ( ref T value );
        public void ForeachClose ( Vector2 pos, float distance, ForeachAction action ) {
            ForeachClose( pos.X, pos.Y, distance, action );
        }
        public void ForeachClose ( float x, float y, float distance, ForeachAction action ) {
            x -= minX;
            y -= minY;
            int xFrom = (int)Math.Clamp( MathF.Floor( ( x - distance ) * RadiusInverse ), 0, maxX );
            int xTo = (int)Math.Clamp( MathF.Floor( ( x + distance ) * RadiusInverse ), 0, maxX );
            int yFrom = (int)Math.Clamp( MathF.Floor( ( y - distance ) * RadiusInverse ), 0, maxY );
            int yTo = (int)Math.Clamp( MathF.Floor( ( y + distance ) * RadiusInverse ), 0, maxY );

            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[ i, j ];
                    var arr = CollectionsMarshal.AsSpan(list);
                    for ( int k = 0; k < list.Count; k++ ) {
                        action(ref arr[k]);
                    }
                }
            }
        }

        public void ForeachClose ( Vector2 pos, float distance, Action<T> action ) {
            ForeachClose( pos.X, pos.Y, distance, action );
        }
        public void ForeachClose ( float x, float y, float distance, Action<T> action ) {
            x -= minX;
            y -= minY;
            int xFrom = (int)Math.Clamp( MathF.Floor( ( x - distance ) * RadiusInverse ), 0, maxX );
            int xTo = (int)Math.Clamp( MathF.Floor( ( x + distance ) * RadiusInverse ), 0, maxX );
            int yFrom = (int)Math.Clamp( MathF.Floor( ( y - distance ) * RadiusInverse ), 0, maxY );
            int yTo = (int)Math.Clamp( MathF.Floor( ( y + distance ) * RadiusInverse ), 0, maxY );

            for ( int i = xFrom; i <= xTo; i++ ) {
                for ( int j = yFrom; j <= yTo; j++ ) {
                    var list = hash[ i, j ];
                    for ( int k = 0; k < list.Count; k++ ) {
                        action( list[ k ] );
                    }
                }
            }
        }
    }
}
