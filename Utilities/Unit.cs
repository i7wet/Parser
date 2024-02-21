using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Utilities;

  [Serializable]
    public struct Unit
        : IEquatable<Unit>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<Unit>
    {
        public static Unit Default = new();
        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Unit"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Unit"/>.</returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Unit;
        }

        /// <summary>Returns a value indicating whether this instance is equal to a specified value.</summary>
        /// <param name="other">An instance to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public bool Equals(Unit other)
        {
            return true;
        }

        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            return other is Unit;
        }

        int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;

            if (other is not Unit)
            {
                throw new ArgumentException("Incorrect type", nameof(other));
            }

            return 0;
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Unit other)
        {
            return 0;
        }

        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;

            if (other is not Unit)
            {
                throw new ArgumentException("Incorrect type", nameof(other));
            }

            return 0;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return 0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return 0;
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Unit"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Unit"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>()</c>.
        /// </remarks>
        public override string ToString()
        {
            return "()";
        }

    public static bool operator ==(Unit left, Unit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Unit left, Unit right)
    {
        return !(left == right);
    }
}