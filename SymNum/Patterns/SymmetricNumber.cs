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
/// </summary>
public class SymmetricNumber : NumberBase
{
    public long UpPosition => Numerator.EndValue;
    public long DownPosition => Denominator.StartValue;
    public long UpUnit => Denominator.EndValue;
    public long DownUnit => Numerator.StartValue;


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
