using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using SymNum.Mind;

namespace SymNum.Measures.Primitives;

// Question:
// Looking at all the information you can get from 1,2,3,4, or 5 bits of information,
//   it is something like(==), (!=, >, <), (ratio), (number by unit), (symmetric number with floating 0 point)
// This suggest values exist that are just 2 bits, just a direction 'greater than something' or 'from the past'
// Our language does this "a lot", "after", "between", "during" all talk of a unspecified place constrained in some partial range.
// So these Extents are not the primitive, but probably made of constraints, and can probably be expressed with bits.
// We can express 40->50 with these Extent ranges, but need to also be able to express 'greater than 40' 
// Even 'between' has a specified start and end, but a path of unspecified length between them, and an unspecified position on that path.
// This needs to be expressible as a value in this system.Old versions used bool segments which works,
//   but it is simpler that those (where each requires 8+ bits of info)
// These segment values then are more like instances, samples using the constraints, that we are trying out in some 'general workspace'

// Aside, we can decompose a segment into<start, path, end>, and convert/imagine it only being <start, path> (direction but no end specified). 
// The mechanics of this operation is divide by zero.
// 40/0 (when you treat zero as below your resolution, but not null) is going to be <greater than 40>
//    (assuming the 0 is actually between 0.0000001, and 0.9999999)
// I suppose it also makes sense to have a -0 (a value we don't measure, but know its direction is negative). Like "worse", "less" etc.

// Either this class needs to be constraint driven to a greater extent, or the range comes from the bits more directly.
// Maybe 2 positions only allows direction to be expressed?
// Really this class should be an analog signal, where a reading is just an uncalibrated value, not an integer with a number system.
// Maybe all numbers live on the properties array, and they need to share units more intentionally?




/// <summary>
/// Represents a span that is uncalibrated so has no meaning. Mathematically, this is a numerator without a denominator.
	/// The ultimate primitive here is a long, but it comes with a lot of baggage that doesn't apply. 
	/// You can have a single measure instead of a span, but then the startpoint is also the endpoint (both are true statements).
/// </summary>
public class Extent : IExtent // was Focal in previous versions
{
    // The reason _positions is an array is there is gradient between an uncalibrated value and an equation.
    // A low res value is like a point, looking closer a range, closer yet might be a curve, later a repeating wave, 
    // and ultimately a specific shape. To separate values from equations is to suggest there isn't a morphism
    // between these transformations.
    // An Extent can only hold samples though, you need some form of 'number/equation' to calibrate them.
    // There can be a single position or more than two positions (but not zero positions which is null),
    // and eventually datapoints can be converted to expressions if a trend is detected. 
    // So Extents can also be purely calculated or stored as an equation.
    // The space of operations that turn a list of samples into equations is anything that separates random from non-random.
    // A list of samples may well be random, but an equation for them assumes they aren't.

    // more likely this isn't needed, and the polarity is set by its position in a number. 
    // probably there is a base array that is a number and it can have 1,2,3,4,5 or more values. These classes then take that 
    // array, sized as need be, and interpret it. Positive is always top right, i unit is top left, zeropoint is fifth etc.

    public bool IsPositiveRight { get; protected set; } = true;
    public bool IsPositiveLeft => !IsPositiveRight;
    /// <summary>
    /// A segment, or partial segment can have a left or right direction. It also can focus on the inside or outside of a segment.
    /// This allows a positive right Extent with only an end point, ExteriorFacing, to represent 'everything after the end point'.
    /// </summary>
    public bool IsInteriorFacing { get; protected set; } = true;
    public bool IsExteriorFacing => !IsInteriorFacing;

    protected Measure EndMeasure { get; private set; }
    protected Measure StartMeasure { get; private set; }

    public long StartPosition => StartMeasure.Position;
    public long EndPosition => EndMeasure.Position;


    //protected long _startPosition;
    //public long StartPosition
    //{
    //    get => _startPosition;
    //    set => _startPosition = value;
    //}
    //protected long _endPosition;
    //public long EndPosition
    //{
    //    get => _endPosition;
    //    set => _endPosition = value;
    //}

    public int AbsDirection => EndMeasure > StartMeasure ? 1 : EndMeasure < StartMeasure ? -1 : 0;
    public bool IsPositiveDirection => IsPositiveRight ? AbsDirection > 0 : AbsDirection < 0;
    public bool IsNegativeDirection => !IsPositiveDirection;
    public bool IsAmbiguousDirection => AbsDirection == 0;
    public long DirectionalMin => IsPositiveDirection ? MinPosition : MaxPosition;
    public long DirectionalMax => IsPositiveDirection ? MaxPosition : MinPosition;

    public long LengthInTicks => (EndMeasure - StartMeasure).Position;
    public long AbsLengthInTicks => Math.Abs(LengthInTicks);
    public long NonZeroLength => LengthInTicks == 0 ? 1 : LengthInTicks;

    public Extent(Measure start, Measure end)
    {
        StartMeasure = start;
        EndMeasure = end;
    }
    public virtual void Reset(Extent extent)
    {
        StartMeasure = extent.StartMeasure;
        EndMeasure = extent.EndMeasure;
    }
    public virtual void Reset(Measure start, Measure end)
    {
        StartMeasure = start;
        EndMeasure = end;
    }

    public virtual long InvertedEndPosition => StartMeasure.Position - LengthInTicks; // todo: check for isMax, isMin
    public Extent Negated => new Extent(-StartMeasure, -EndMeasure);
    public void MirrorAroundStart()
    {
        EndMeasure.SetPosition(StartMeasure.Position - LengthInTicks); // todo: check for isMax, isMin
    }
    public void Reverse()
    {
        var temp = StartMeasure;
        StartMeasure = EndMeasure;
        EndMeasure = temp;
    }
    public void MakeForward()
    {
        if (EndMeasure < StartMeasure)
        {
            Reverse();
        }
    }


    public bool Touches(Extent q) => Touches(this, q);

    public Measure MinMeasure => Measure.Min(StartMeasure, EndMeasure);
    public Measure MaxMeasure => Measure.Max(StartMeasure, EndMeasure);
    public long MinPosition => Measure.MinPosition(StartMeasure, EndMeasure);
    public long MaxPosition => Measure.MaxPosition(StartMeasure, EndMeasure);
    public static Measure MinStartMeasure(Extent p, Extent q) => Measure.Min(p.StartMeasure, q.StartMeasure);
    public static Measure MaxStartMeasure(Extent p, Extent q) => Measure.Max(p.StartMeasure, q.StartMeasure);
    public static Measure MinEndMeasure(Extent p, Extent q) => Measure.Min(p.EndMeasure, q.EndMeasure);
    public static Measure MaxEndMeasure(Extent p, Extent q) => Measure.Max(p.EndMeasure, q.EndMeasure);
    public static long MinExtentPosition(Extent p, Extent q) => Math.Min(p.MinPosition, q.MinPosition);
    public static long MaxExtentPosition(Extent p, Extent q) => Math.Max(p.MaxPosition, q.MaxPosition);
    public static long MinStartPosition(Extent p, Extent q) => Measure.MinPosition(p.StartMeasure, q.StartMeasure);
    public static long MaxStartPosition(Extent p, Extent q) => Measure.MaxPosition(p.StartMeasure, q.StartMeasure);
    public static long MinEndPosition(Extent p, Extent q) => Measure.MinPosition(p.EndMeasure, q.EndMeasure);
    public static long MaxEndPosition(Extent p, Extent q) => Measure.MaxPosition(p.EndMeasure, q.EndMeasure);

    // todo: Pretty sure these bool ops need updating with the partial measures. Just making compile for now.

    /// <summary>
    /// Shares an endpoint, but no overlap (meaning can only share one endpoint).
    /// </summary>
    public static bool Touches(Extent p, Extent q) => p.EndMeasure == q.StartMeasure || p.StartMeasure == q.EndMeasure;
    public static Extent? Intersection(Extent p, Extent q)
    {
        Extent? result = null;
        var ov = Overlap(p, q);
        if (ov.LengthInTicks != 0)
        {
            result = ov;
            if (!p.IsPositiveDirection) { ov.Reverse(); }
        }
        return result;
    }
    public static Extent Overlap(Extent p, Extent q)
    {
        var start = Measure.Max(p.MinMeasure, q.MinMeasure);
        var end = Measure.Min(p.MaxMeasure, q.MaxMeasure);
        return start >= end ? new Extent(Measure.Unknown, Measure.Unknown) : new Extent(start, end);
    }
    public static Extent Join(Extent p, Extent q)
    {
        var start = Measure.Min(p.MinMeasure, q.MinMeasure);
        var end = Measure.Max(p.MaxMeasure, q.MaxMeasure);
        return new Extent(start, end);
    }

    // Q. Should direction be preserved in a bool operation?
    // A.  No. You can filter them to separate or manually unify direction. These should unify direction to align with the... existing class? 
    public static Extent[] Never(Extent p)
    {
        return new Extent[0];
    }
    public static Extent[] UnaryNot(Extent p)
    {
        // If p starts at the beginning of the time frame and ends at the end, A is always true and the "not A" relationship is empty
        if (p.StartMeasure.IsZero && p.EndMeasure.IsMax)
        {
            return new Extent[] { };
        }
        // If p starts at the beginning of the time frame and ends before the end, the "not A" relationship consists of a single interval from p.EndTickPosition + 1 to the end of the time frame
        else if (p.StartMeasure.IsZero)
        {
            return new Extent[] { new Extent(p.EndMeasure++, Measure.MaxDown()) };
        }
        // If p starts after the beginning of the time frame and ends at the end, the "not A" relationship consists of a single interval from the beginning of the time frame to p.StartTickPosition - 1
        else if (p.EndMeasure.IsMax)
        {
            return new Extent[] { new Extent(Measure.ZeroUp(), p.StartMeasure--) };
        }
        // If p starts and ends within the time frame, the "not A" relationship consists of two intervals: from the beginning of the time frame to p.StartTickPosition - 1, and from p.EndTickPosition + 1 to the end of the time frame
        else
        {
            return new Extent[] {
                new Extent(Measure.ZeroUp(), p.StartMeasure--),
                new Extent(p.EndMeasure++, Measure.MaxDown()) };
        }
    }
    public static Extent[] Transfer(Extent p)
    {
        return new Extent[] { p.Clone() };
    }
    public static Extent[] Always(Extent p)
    {
        return new Extent[] { MinMaxExtent.Clone() };
    }

    public static Extent[] Never(Extent p, Extent q)
    {
        return new Extent[0];
    }
    public static Extent[] And(Extent p, Extent q)
    {
        var overlap = Overlap(p, q);
        return overlap.LengthInTicks == 0 ? new Extent[0] : new Extent[] { overlap };
    }
    public static Extent[] B_Inhibits_A(Extent p, Extent q)
    {
        if (p.EndMeasure < q.StartMeasure-- || q.EndMeasure < p.StartMeasure--)
        {
            return new Extent[] { p };
        }
        else
        {
            return new Extent[] { new Extent(p.StartMeasure, q.StartMeasure--) };
        }
    }
    public static Extent[] Transfer_A(Extent p, Extent q)
    {
        return new Extent[] { p };
    }
    public static Extent[] A_Inhibits_B(Extent p, Extent q)
    {
        if (p.EndMeasure < q.StartMeasure-- || q.EndMeasure < p.StartMeasure--)
        {
            return new Extent[] { q };
        }
        else
        {
            return new Extent[] { new Extent(q.StartMeasure, p.StartMeasure--) };
        }
    }
    public static Extent[] Transfer_B(Extent p, Extent q)
    {
        return new Extent[] { q };
    }
    public static Extent[] Xor(Extent p, Extent q)
    {
        // Return the symmetric difference of the two input segments as a new array of segments
        List<Extent> result = new List<Extent>();
        Extent[] andResult = And(p, q);
        if (andResult.Length == 0)
        {
            // If the segments do not intersect, return the segments as separate non-overlapping segments
            result.Add(p);
            result.Add(q);
        }
        else
        {
            // If the segments intersect, return the complement of the intersection in each segment
            Extent[] complement1 = Nor(p, andResult[0]);
            Extent[] complement2 = Nor(q, andResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Extent[] Or(Extent p, Extent q)
    {
        var overlap = Overlap(p, q);
        return overlap.LengthInTicks == 0 ? new Extent[] { p, q } : new Extent[] { Join(p, q) };
    }
    public static Extent[] Nor(Extent p, Extent q)
    {
        // Return the complement of the union of the two input Extents as a new array of Extents
        List<Extent> result = new List<Extent>();
        Extent[] orResult = Or(p, q);
        if (orResult.Length == 0)
        {
            // If the Extents do not overlap, return both Extents as separate non-overlapping Extents
            result.Add(p);
            result.Add(q);
        }
        else
        {
            // If the Extents overlap, return the complement of the union in each Extent
            Extent[] complement1 = Nand(p, orResult[0]);
            Extent[] complement2 = Nand(q, orResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Extent[] Xnor(Extent p, Extent q)
    {
        // Return the complement of the symmetric difference of the two input Extents as a new array of Extents
        List<Extent> result = new List<Extent>();
        Extent[] xorResult = Xor(p, q);
        if (xorResult.Length == 0)
        {
            // If the Extents are equal, return p as a single Extent
            result.Add(p);
        }
        else
        {
            // If the Extents are not equal, return the complement of the symmetric difference in each Extent
            Extent[] complement1 = Nor(p, xorResult[0]);
            Extent[] complement2 = Nor(q, xorResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Extent[] Not_B(Extent p, Extent q)
    {
        return UnaryNot(q);
    }
    public static Extent[] B_Implies_A(Extent p, Extent q)
    {
        if (p.EndMeasure < q.StartMeasure-- || q.EndMeasure < p.StartMeasure--)
        {
            return new Extent[] { };
        }
        else
        {
            return new Extent[] { new Extent(MaxStartMeasure(p, q), MinEndMeasure(p, q)) };
        }
    }
    public static Extent[] Not_A(Extent p, Extent q)
    {
        return UnaryNot(p);
    }
    public static Extent[] A_Implies_B(Extent p, Extent q)
    {
        if (p.EndMeasure < q.StartMeasure-- || q.EndMeasure < p.StartMeasure--)
        {
            return new Extent[] { };
        }
        else
        {
            return new Extent[] { new Extent(MaxStartMeasure(p, q), MinEndMeasure(p, q)) };
        }
    }
    public static Extent[] Nandx(Extent p, Extent q)
    {
        // Return the complement of the intersection of the two input Extents as a new array of Extents
        List<Extent> result = new List<Extent>();
        Extent[] andResult = And(p, q);
        if (andResult.Length == 0)
        {
            // If the Extents do not intersect, return the union of the Extents as a single Extent
            result.Add(new Extent(MinStartMeasure(p, q), MaxEndMeasure(p, q)));
        }
        else
        {
            // If the Extents intersect, return the complement of the intersection in each Extent
            Extent[] complement1 = Nor(p, andResult[0]);
            Extent[] complement2 = Nor(q, andResult[0]);
            result.AddRange(complement1);
            result.AddRange(complement2);
        }
        return result.ToArray();
    }
    public static Extent[] Nand(Extent p, Extent q)
    {
        Extent[] result;
        var overlap = Overlap(p, q);
        if (overlap.LengthInTicks == 0)
        {
            result = new Extent[] { MinMaxExtent.Clone() };
        }
        else
        {
            result = new Extent[]
            {
            new Extent(Measure.MinUp(), overlap.StartMeasure),
            new Extent(overlap.EndMeasure, Measure.MaxDown())
            };
        }
        return result;
    }
    public static Extent[] Always(Extent p, Extent q)
    {
        return new Extent[] { MinMaxExtent.Clone() };
    }

    //  0000	Never			0			FALSE
    //  0001	Both            A ^ B       AND
    //  0010	Only A          A ^ !B      A AND NOT B
    //  0011	A Maybe B       A           A
    //  0100	Only B			!A ^ B      NOT A AND B
    //  0101	B Maybe A       B           B
    //  0110	One of          A xor B     XOR
    //  0111	At least one    A v B       OR
    //  1000	No one          A nor B     NOR
    //  1001	Both or no one  A XNOR B    XNOR
    //  1010	A or no one		!B          NOT B
    //  1011	Not B alone     A v !B      A OR NOT B
    //  1100	B or no one		!A          NOT A
    //  1101	Not A alone		!A v B      NOT A OR B
    //  1110	Not both        A nand B    NAND
    //  1111	Always			1			TRUE

    public long Never(long a) { return 0; } // Null
    public long Transfer(long a) { return a; }
    public long UnaryNot(long a) { return ~a; }
    public long Identity(long a) { return -1; }

    public long Never(long a, long b) { return 0; } // Null
    public long And(long a, long b) { return a & b; }
    public long B_Inhibits_A(long a, long b) { return a & ~b; } // inhibition, div a/b. ab`
    public long Transfer_A(long a, long b) { return a; }                   // transfer
    public long A_Inhibits_B(long a, long b) { return ~a & b; } // inhibition, div b/a. a`b
    public long Transfer_B(long a, long b) { return b; }                   // transfer
    public long Xor(long a, long b) { return a ^ b; }
    public long Or(long a, long b) { return a | b; }
    public long Nor(long a, long b) { return ~(a | b); }
    public long Xnor(long a, long b) { return ~(a ^ b); } // equivalence, ==. (xy)`
    public long Not_B(long a, long b) { return ~b; } // complement
    public long B_Implies_A(long a, long b) { return a | ~b; } // implication (Not B alone)
    public long Not_A(long a, long b) { return ~a; } // complement
    public long A_Implies_B(long a, long b) { return b | ~a; } // implication (Not A alone)
    public long Nand(long a, long b) { return ~(a & b); }
    public long Always(long a, long b) { return -1; }


    //public static Extent CreateZeroExtent(long ticks) { return new Extent(0, ticks); }
    //public static Extent CreateBalancedExtent(long halfTicks) { return new Extent(-halfTicks, halfTicks); }
    public static Extent MinMaxExtent => new Extent(Measure.MinUp(), Measure.MaxDown());
    //public static Extent OneExtent => new Extent(0, 1);
    //public static Extent UpMaxExtent => new Extent(0, long.MaxValue);

    /// <summary>
    /// Will be true other than MaskedFocals, where a bool operation can start with false (like with an empty result).
    /// </summary>
    public BoolState StartState { get; protected set; } = BoolState.True;

    public virtual long[] GetPositions() => [StartPosition, EndPosition];
    public virtual void SetPosition(int index, long value)
    {
        if (index == 0)
        {
            StartMeasure.SetPosition(value);
        }
        else if (index == 1)
        {
            EndMeasure.SetPosition(value);
        }
    }
    public virtual void ClearInternalPositions() { }
    /// <summary>
    /// Creates table with all stops, giving left and right position states for each stop.
    /// A truth table only acts on valid parts of segments. -10i+5 has two parts, 0 to -10i and 0 to 5. This is the area bools apply to.
    /// </summary>
    public static List<(long, BoolState, BoolState)> BuildTruthTable(Extent left, Extent right)
    {
        var result = new List<(long, BoolState, BoolState)>();
        var leftPositions = left.GetPositions();
        var rightPositions = right.GetPositions();
        if (leftPositions.Length > 0)
        {
            var sortedAll = new SortedSet<long>(leftPositions);
            sortedAll.UnionWith(rightPositions);
            var leftSideState = left.StartState.Invert(); // start in the pre segment area, so opposite state
            var rightSideState = right.StartState.Invert();
            int index = 0;
            foreach (var pos in sortedAll)
            {
                if (leftPositions.Contains(pos)) { leftSideState = leftSideState.Invert(); }
                if (rightPositions.Contains(pos)) { rightSideState = rightSideState.Invert(); }
                result.Add((pos, leftSideState, rightSideState));
                index++;
            }
        }
        return result;
    }
    /// <summary>
    /// Runs bool op over truth table (all stop positions of two sets, with the boolState for each side).
    /// </summary>
    public static long[] ApplyOpToTruthTable(List<(long, BoolState, BoolState)> data, Func<bool, bool, bool> operation)
    {
        var result = new List<long>();
        var lastResult = false;
        var hadFirstTrue = false;
        for (int i = 0; i < data.Count - 1; i++)
        {
            var item = data[i];
            var valid = BoolStateExtension.AreBool(item.Item2, item.Item3);
            var opResult = operation(item.Item2.BoolValue(), item.Item3.BoolValue());
            if (!hadFirstTrue && opResult == true)
            {
                result.Add(item.Item1);
                hadFirstTrue = true;
                lastResult = opResult;

            }
            else if (lastResult != opResult)
            {
                result.Add(item.Item1);
                lastResult = opResult;
            }
        }

        if (lastResult == true && result.Count > 0) // always close
        {
            result.Add(data.Last().Item1);
        }
        return result.ToArray();
    }

    #region Equality
    public virtual Extent Clone()
    {
        return new Extent(StartMeasure, EndMeasure);
    }
    public override bool Equals(object? obj)
    {
        return obj is Extent other && Equals(other);
    }
    public bool Equals(Extent? value)
    {
        return ReferenceEquals(this, value) || value != null && StartMeasure.Equals(value.StartMeasure) && EndMeasure.Equals(value.EndMeasure);
    }

    public static bool operator ==(Extent? a, Extent? b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        if (a is null || b is null)
        {
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Extent? a, Extent? b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = StartMeasure.GetHashCode();
            hashCode = hashCode * 397 ^ EndMeasure.GetHashCode();
            return hashCode;
        }
    }
    #endregion
    public override string ToString() => $"[{StartMeasure} : {EndMeasure}]";
}
