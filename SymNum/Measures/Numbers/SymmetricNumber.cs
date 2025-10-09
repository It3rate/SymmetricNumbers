using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Numbers;

/// <summary>
/// A symmetric number is made of four measures, where nothing is fixed at zero or 'unit'.
/// The operations of this allow stretching or negating both the unit and the value, allowing transforms to different 'number systems'.
/// This makes two variable units, which can describe transforms that negate and mirror, but also restrict those.
/// The potential contradictions can be resolved by mapping to an extended dimension (spatial, time, other).
/// This allows a line to have concepts like left or right of the line in a given direction - 
/// a concept requiring the existance of some 'y' dimension, but fully describable in the 1D 'x' dimension.
/// See: https://x.com/debreuil/status/1876646280451051933
/// </summary>
public class SymmetricNumber : UnconstrainedNumber
{
    public long TopRight { get => RightMeasure.Position; protected set => RightMeasure.SetPosition(value); }
    public long TopLeft { get => LeftUnit.Position; protected set => LeftUnit.SetPosition(value); }
    public long BottomRight { get => RightUnit.Position; protected set => RightUnit.SetPosition(value); }
    public long BottomLeft { get => LeftMeasure.Position; protected set => LeftMeasure.SetPosition(value); }

    public bool HasTR => RightMeasure.IsFullDefined;
    public bool HasTL => LeftUnit.IsFullDefined;
    public bool HasBR => RightUnit.IsFullDefined;
    public bool HasBL => LeftMeasure.IsFullDefined;
    private int _totalNonZero => (HasTR ? 1 : 0) + (HasTL ? 1 : 0) + (HasBR ? 1 : 0) + (HasBL ? 1 : 0);

    public bool IsHalt => _totalNonZero == 0; // empty
    /// <summary>
    /// only larger than (no value known)
    /// </summary>
    public bool IsTransform => _totalNonZero == 1; // point
    /// <summary>
    /// scale the up or down values.
    /// </summary>
    public bool IsMove => _totalNonZero == 2 && HasTR == HasBR; // vertical 
    /// <summary>
    /// change the 'somewhere between' scale, don't know the actual position or calibrated values.
    /// </summary>
    public bool IsLine => _totalNonZero == 2 && HasTR == HasBL; // diagonal
    /// <summary>
    /// Change down resolution, up value, or vice versa. 
    /// Adjusts certainty of how far below resolution one is, and 'at least how large' the other is.
    /// Increasing values increases the certainty they aren't touching. (eg at least more than 40 -> at least less than 0.001)
    /// </summary>
    public bool IsSplitJoin => _totalNonZero == 2 && HasTR == HasTL; // horizontal 
    /// <summary>
    /// 
    /// </summary>
    public bool IsIfTurn => _totalNonZero == 3; // corner
    /// <summary>
    /// 
    /// </summary>
    public bool IsContinue => _totalNonZero == 4; // full
    public long RightResolution => RightUnit.Position;
    public long LeftResolution => LeftUnit.Position;
    public long MinResolution => Math.Min(RightResolution, LeftResolution);
    public long MaxResolution => Math.Max(RightResolution, LeftResolution);

    public SymmetricNumber(long tr, long tl, long br, long bl)
    {
        RightMeasure = new Measure(tr, Direction.FacesRight, true);
        LeftUnit = new Measure(tl, Direction.FacesLeft, true);
        RightUnit = new Measure(br, Direction.FacesRight, true);
        LeftMeasure = new Measure(bl, Direction.FacesLeft, true);
    }
    public SymmetricValue Symmetric => new SymmetricValue(this);
    public class SymmetricValue : NumberValue
    {
        public SymmetricNumber Reference { get; }
        public float RightValue => UnconstrainedNumber.GetValue(Reference.RightMeasure, Reference.RightUnit);
        public float LeftValue => UnconstrainedNumber.GetValue(Reference.LeftMeasure, Reference.LeftUnit);
        public float GetValue(Measure measure) => measure.IsFullDefined ? measure.Position : 0;
        public float GetValue(Measure numerator, Measure denominator) => UnconstrainedNumber.GetValue(numerator, denominator);
        // todo: public float GetValue(Measure a, Measure b, Measure c) => ;
        // todo: public float GetValue(Measure a, Measure b, Measure c, Measure d) => ;
        public override long RightResolution => Reference.RightResolution;
        public override long LeftResolution => Reference.LeftResolution;
        public SymmetricValue(SymmetricNumber baseNumber) : base(baseNumber)
        {
            Reference = baseNumber;
        }
    }
}
