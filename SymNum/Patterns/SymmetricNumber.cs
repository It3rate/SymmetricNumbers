using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

/// <summary>
/// A symmetric number is made of two extents, where neither is fixed at zero, and neither has a set direction.
/// This makes two variable units, which can describe transforms that negate and mirror, but also restrict those.
/// The potential contradictions can be resolved by mapping to an extended dimension (spatial, time, other).
/// This allows a line to have concepts like left or right of the line in a given direction - 
/// a concept requiring the existance of some 'y' dimension, but fully describable in the 1D 'x' dimension.
/// See: https://x.com/debreuil/status/1876646280451051933
/// </summary>
public class SymmetricNumber : NumberBase
{
    public long UpPosition => Numerator.EndValue;
    public long DownPosition => Denominator.StartValue;
    public long UpUnit => Denominator.EndValue;
    public long DownUnit => Numerator.StartValue;

    public bool HasTR => UpPosition != 0;
    public bool HasTL => DownUnit != 0;
    public bool HasBR => UpUnit != 0;
    public bool HasBL => DownPosition != 0;
    private int _totalNonZero => (HasTR ? 1 : 0) + (HasTL ? 1 : 0) + (HasBR ? 1 : 0) + (HasBL ? 1 : 0);

    public bool IsHalt => _totalNonZero == 0; // empty
    /// <summary>
    /// only larger than (no value known)
    /// </summary>
    public bool IsTransform => _totalNonZero == 1; // point
    /// <summary>
    /// scale the up or down values.
    /// </summary>
    public bool IsMove => _totalNonZero == 2 && (HasTR == HasBR); // vertical 
    /// <summary>
    /// change the 'somewhere between' scale, don't know the actual position or calibrated values.
    /// </summary>
    public bool IsLine => _totalNonZero == 2 && (HasTR == HasBL); // diagonal
    /// <summary>
    /// Change down resolution, up value, or vice versa. 
    /// Adjusts certainty of how far below resolution one is, and 'at least how large' the other is.
    /// Increasing values increases the certainty they aren't touching. (eg at least more than 40 -> at least less than 0.001)
    /// </summary>
    public bool IsSplitJoin => _totalNonZero == 2 && (HasTR == HasTL); // horizontal 
    /// <summary>
    /// 
    /// </summary>
    public bool IsIfTurn => _totalNonZero == 3; // corner
    /// <summary>
    /// 
    /// </summary>
    public bool IsContinue => _totalNonZero == 4; // full

    public SymmetricNumber(long tr, long tl, long br, long bl)
    {
        Numerator = new Extent(tl, tr, false); // there isn't really a positive right or left as all values are relative to each other.
        Denominator = new Extent(bl, br, true);
    }
    public SymmetricValue SymmetricValue => new SymmetricValue(this);
}
public class SymmetricValue : NumberValue
{
    public SymmetricNumber Reference { get; }
    public float Value => (MidPosition - StartPosition) / (float)(EndPosition - StartPosition);
    public override long UpResolution => Reference.Denominator.EndValue;
    public override long DownResolution => Reference.Numerator.StartValue;
    private long StartPosition => Reference.Denominator.StartValue;
    private long MidPosition => Reference.Numerator.EndElseStartValue;
    private long EndPosition => Reference.Denominator.EndValue;
    public SymmetricValue(SymmetricNumber baseNumber) : base(baseNumber)
    {
        Reference = baseNumber;
    }
}
