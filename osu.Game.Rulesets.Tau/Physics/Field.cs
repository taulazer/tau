using osu.Game.Rulesets.Objects.Types;
using osuTK;
using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Tau.Physics {
    // https://www.plymouth.ac.uk/uploads/production/document/path/3/3744/PlymouthUniversity_MathsandStats_the_Laplacian.pdf
    // https://activecalculus.org/vector/S_Vector_VectorFields.html
    public static class Field {
        public delegate float ScalarFn ( Vector2 p );
        public delegate Vector2 VectorFn ( Vector2 p );

        /// <summary>
        /// Denoted ∇f(x,y) where f: (R, R) -> R.
        /// Computes the partial derivative along the 2 axis.
        /// </summary>
        /// <remarks>
        /// This is equivalent to (∂f/∂x, ∂f/∂y)
        /// </remarks>
        /// <param name="s">The numerical precision of the derivatives</param>
        /// <returns>F(x, y) where F: (R, R) -> (R, R)</returns>
        public static VectorFn Gradient ( ScalarFn fn, float s = 0.001f ) {
            var sInv = 1 / s / 2;
            return p => {
                var dx = (fn(p + new Vector2(s, 0)) - fn(p - new Vector2(s, 0))) * sInv;
                var dy = (fn(p + new Vector2(0, s)) - fn(p - new Vector2(0, s))) * sInv;

                return new(dx, dy);
            };
        }

        /// <summary>
        /// Denoted ∇ · F(x,y) where F: (R, R) -> (R, R).
        /// Computes the change in vector strength along the 2 axis.
        /// </summary>
        /// <remarks>
        /// This is equivalent to ∂F.x/∂x + ∂F.y/∂y
        /// </remarks>
        /// <param name="s">The numerical precision of the derivatives</param>
        /// <returns>f(x, y) where f: (R, R) -> R</returns>
        public static ScalarFn Divergence ( VectorFn fn, float s = 0.001f ) {
            var sInv = 1 / s / 2;

            return p => {
                var dx = (fn(p + new Vector2(s, 0)).X - fn(p - new Vector2(s, 0)).X) * sInv;
                var dy = (fn(p + new Vector2(0, s)).Y - fn(p - new Vector2(0, s)).Y) * sInv;

                return dx + dy;
            };
        }

        /// <summary>
        /// Denoted ∇ × F(x,y) where F: (R, R) -> (R, R).
        /// Computes the rotation at a given point on a vector field.
        /// </summary>
        /// <remarks>
        /// This is equivalent to ∂F.y/∂x - ∂F.x/∂y
        /// </remarks>
        /// <param name="s">The numerical precision of the derivatives</param>
        /// <returns>f(x, y) where f: (R, R) -> R</returns>
        public static ScalarFn Curl ( VectorFn fn, float s = 0.001f ) {
            var sInv = 1 / s / 2;

            return p => {
                var dy_dx = ( fn( p + new Vector2( s, 0 ) ).Y - fn( p - new Vector2( s, 0 ) ).Y ) * sInv;
                var dx_dy = ( fn( p + new Vector2( 0, s ) ).X - fn( p - new Vector2( 0, s ) ).X ) * sInv;

                return dy_dx - dx_dy;
            };
        }

        /// <summary>
        /// Denoted (∇ · ∇, ∇^2 or Δ) f(x,y) where f: (R, R) -> R.
        /// Computes the divergence of the gradient of a scalar field.
        /// </summary>
        /// <remarks>
        /// This is equivalent to ∂^2f/∂x^2 + ∂^2f/∂y^2
        /// </remarks>
        /// <param name="s">The numerical precision of the derivatives</param>
        /// <returns>f(x, y) where f: (R, R) -> R</returns>
        public static ScalarFn Laplacian ( ScalarFn fn, float s = 0.001f ) {
            var sInv = 1 / s;

            return p => {
                var k = fn(p);

                var xp = (fn(p + new Vector2(s, 0)) - k) * sInv;
                var xn = (k - fn(p - new Vector2(s, 0))) * sInv;
                var dx = (xp - xn) * sInv;

                var yp = (fn(p + new Vector2(0, s)) - k) * sInv;
                var yn = (k - fn(p - new Vector2(0, s))) * sInv;
                var dy = (yp - yn) * sInv;

                return dx + dy;
            };
        }

        /// <summary>
        /// Denoted (∇ · ∇, ∇^2 or Δ) F(x,y) where F: (R, R) -> (R, R).
        /// Computes divergence of the gradient of the components of the vector field.
        /// </summary>
        /// <remarks>
        /// This is equivalent to (∇^2 F.x, ∇^2 F.y)
        /// </remarks>
        /// <param name="s">The numerical precision of the derivatives</param>
        /// <returns>F(x, y) where F: (R, R) -> (R, R)</returns>
        public static VectorFn Laplacian ( VectorFn fn, float s = 0.001f ) {
            var sInv = 1 / s;

            return p => {
                var k = fn(p);

                var xp = (fn(p + new Vector2(s, 0)) - k) * sInv;
                var xn = (k - fn(p - new Vector2(s, 0))) * sInv;
                var dx = (xp - xn) * sInv;

                var yp = (fn(p + new Vector2(0, s)) - k) * sInv;
                var yn = (k - fn(p - new Vector2(0, s))) * sInv;
                var dy = (yp - yn) * sInv;

                return dx + dy;
            };
        }

        public static ScalarFn KernelBell ( float h = 1 ) {
            var factor = 315f / 64 / MathF.PI / MathF.Pow( h, 6 );
            var h2 = h * h;

            return p => {
                var r2 = p.LengthSquared;
                var n = h2 - r2;

                return r2 <= h2
                    ? factor * n * n * n
                    : 0;
            };
        }
        public static ScalarFn KernelSpike ( float h = 1 ) {
            var factor = 15f / MathF.PI / MathF.Pow( h, 6 );

            return p => {
                var r = p.LengthFast;
                var n = h - r;

                return r <= h
                    ? factor * n * n * n
                    : 0;
            };
        }
        public static VectorFn KernelSpikeGradient ( float h = 1 ) {
            var factor = -45f / MathF.PI / MathF.Pow( h, 6 );

            return p => {
                var r = p.LengthFast;

                return r > 0 && r <= h
                    ? factor * p * ( h*h/r - 2*h + r )
                    : Vector2.Zero;
            };
        }
        public static ScalarFn KernelAsymptote ( float h = 1 ) {
            var h2 = h * h;
            var h3 = h2 * h;
            var factor = 15f / 2 / MathF.PI / h3;

            return p => {
                var r = p.LengthFast;
                var r2 = r * r;
                var r3 = r * r2;

                return r <= h
                    ? factor * ( -r3 / 2 / h3 + r2 / h2 + h / 2 / r - 1 )
                    : 0;
            };
        }
    }

    public static class ParticleField<T> where T : IHasPosition {
        public static float FieldAt ( Vector2 position, IEnumerable<T> particles, Func<T, float> selector, Field.ScalarFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            float value = 0;
            foreach ( var particle in particles ) {
                value += selector( particle ) * kernel( position - particle.Position * scaleInv );
            }

            return value;
        }
        public static float FieldAt ( Vector2 position, Span<T> particles, Func<T, float> selector, Field.ScalarFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            float value = 0;
            for ( int i = 0; i < particles.Length; i++ ) {
                var particle = particles[i];
                value += selector( particle ) * kernel( position - particle.Position * scaleInv );
            }

            return value;
        }

        public static Vector2 FieldAt ( Vector2 position, IEnumerable<T> particles, Func<T, Vector2> selector, Field.ScalarFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            Vector2 value = Vector2.Zero;
            foreach ( var particle in particles ) {
                value += selector( particle ) * kernel( position - particle.Position * scaleInv );
            }

            return value;
        }
        public static Vector2 FieldAt ( Vector2 position, Span<T> particles, Func<T, Vector2> selector, Field.ScalarFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            Vector2 value = Vector2.Zero;
            for ( int i = 0; i < particles.Length; i++ ) {
                var particle = particles[i];
                value += selector( particle ) * kernel( position - particle.Position * scaleInv );
            }

            return value;
        }

        public static float FieldAt ( Vector2 position, IEnumerable<T> particles, Func<T, Vector2> selector, Field.ScalarFn kernel, Func<Field.VectorFn, Field.ScalarFn> transformer, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            float value = 0;
            foreach ( var particle in particles ) {
                var pos = position - particle.Position * scaleInv;

                value += transformer( p => selector( particle ) * kernel( p ) )( pos );
            }

            return value;
        }
        public static float FieldAt ( Vector2 position, Span<T> particles, Func<T, Vector2> selector, Field.ScalarFn kernel, Func<Field.VectorFn, Field.ScalarFn> transformer, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            float value = 0;
            for ( int i = 0; i < particles.Length; i++ ) {
                var particle = particles[ i ];
                var pos = position - particle.Position * scaleInv;

                value += transformer( p => selector( particle ) * kernel( p ) )( pos );
            }

            return value;
        }

        public static Vector2 FieldAt ( Vector2 position, IEnumerable<T> particles, Func<T, float> selector, Field.VectorFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            Vector2 value = Vector2.Zero;
            foreach ( var particle in particles ) {
                value += selector( particle ) * kernel( position - particle.Position * scaleInv );
            }

            return value;
        }
        public static Vector2 FieldAt ( Vector2 position, Span<T> particles, Func<T, float> selector, Field.VectorFn kernel, float scale = 1 ) {
            var scaleInv = 1 / scale;
            position *= scaleInv;

            Vector2 value = Vector2.Zero;
            for ( int i = 0; i < particles.Length; i++ ) {
				var particle = particles[ i ];
				value += selector( particle ) * kernel( position - particle.Position * scaleInv );
			}

            return value;
        }
    }
}
