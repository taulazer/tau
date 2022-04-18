using System;
using System.Buffers;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Tau.Allocation
{
    /// <summary>
    /// Wrapper for <see cref="ArrayPool{T}"/> which returns <see cref="RentedArray{T}"/>
    /// and an optional rented <see cref="Span{T}"/> or <see cref="Memory{T}"/>.
    /// </summary>
    /// <remarks>
    /// <code>
    /// using (MemoryPool&lt;T&gt;.Shared.Rent(100, out Span&lt;T&gt; span)) { ... }
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class MemoryPool<T>
    {
        private readonly ArrayPool<T> backing;

        public MemoryPool(ArrayPool<T> backing)
        {
            this.backing = backing;
        }

        public static MemoryPool<T> Shared { get; } = new(ArrayPool<T>.Shared);

        public RentedArray<T> Rent(ICollection<T> value)
        {
            var arr = backing.Rent(value.Count);
            value.CopyTo(arr, 0);
            return new RentedArray<T>(backing, arr, value.Count);
        }
    }

    public struct RentedArray<T> : IDisposable
    {
        private readonly ArrayPool<T> backing;
        private readonly T[] rented;
        public readonly int Length;

        public RentedArray(ArrayPool<T> backing, T[] rented, int length)
        {
            this.backing = backing;
            this.rented = rented;
            Length = length;
        }

        public ref T this[int i] => ref rented[i];
        public ref T this[Index i] => ref rented[i];

        public void Dispose()
        {
            backing?.Return(rented);
        }
    }
}
