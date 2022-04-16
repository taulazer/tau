using System;
using System.Collections.Generic;

public class MemoryPool<T> {
    private System.Buffers.ArrayPool<T> backing;
    public MemoryPool ( System.Buffers.ArrayPool<T> backing ) {
        this.backing = backing;
    }

    public static MemoryPool<T> Shared { get; } = new MemoryPool<T>( System.Buffers.ArrayPool<T>.Shared );

    public RentedArray<T> Rent ( int size ) {
        var arr = backing.Rent( size );
        return new RentedArray<T>( backing, arr, size );
    }
    public RentedArray<T> Rent ( ICollection<T> value ) {
        var arr = backing.Rent( value.Count );
        value.CopyTo( arr, 0 );
        return new RentedArray<T>( backing, arr, value.Count );
    }

    public RentedArray<T> Rent ( int size, out Span<T> span ) {
        var arr = backing.Rent( size );
        span = arr.AsSpan(0, size);
        return new RentedArray<T>( backing, arr, size );
    }
    public RentedArray<T> Rent ( ICollection<T> value, out Span<T> span ) {
        var arr = backing.Rent( value.Count );
        value.CopyTo( arr, 0 );
        span = arr.AsSpan( 0, value.Count );
        return new RentedArray<T>( backing, arr, value.Count );
    }

    public RentedArray<T> RentMemory ( int size, out Memory<T> memory ) {
        var arr = backing.Rent( size );
        memory = arr.AsMemory( 0, size );
        return new RentedArray<T>( backing, arr, size );
    }
    public RentedArray<T> RentMemory ( ICollection<T> value, out Memory<T> memory ) {
        var arr = backing.Rent( value.Count );
        value.CopyTo( arr, 0 );
        memory = arr.AsMemory( 0, value.Count );
        return new RentedArray<T>( backing, arr, value.Count );
    }
}

public struct RentedArray<T> : IDisposable {
    private System.Buffers.ArrayPool<T> backing;
    private T[] rented;
    public readonly int Length;

    public RentedArray ( System.Buffers.ArrayPool<T> backing, T[] rented, int length ) {
        this.backing = backing;
        this.rented = rented;
        Length = length;
    }

    public Span<T>.Enumerator GetEnumerator () => rented.AsSpan( 0, Length ).GetEnumerator();
    public ref T this[int i] => ref rented[i];
    public ref T this[Index i] => ref rented[i];

    public void Dispose () {
        backing?.Return( rented );
    }
}
