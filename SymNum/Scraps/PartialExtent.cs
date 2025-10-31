using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Scraps;

/// <summary>
/// A number that isn't fully formed. This might look like 'greater than 3', positive, 
/// the imaginary part of a complex number, n/0. There are four samples for a fully formed number,
/// this has at least one of those sample missing, or only expressed as a contraint (less than a different sample etc).
/// </summary>
public class PartialExtent : IExtent
{
    private long? _startValue;
    private long? _endValue;
    private Comparator _direction = Comparator.Unknown;
    public PartialExtent(long value, bool isStart)
    {
        _startValue = isStart ? value : null;
        _endValue = !isStart ? value : null;
    }
    public PartialExtent(long value, bool isStart, Comparator direction) : this(value, isStart)
    {
        _startValue = isStart ? value : null;
        _endValue = !isStart ? value : null;
        _direction = direction;
    }

    public long EndElseStartValue => _endValue ?? _startValue!.Value;
    public long StartElseEndValue => _startValue ?? _endValue!.Value;
    public bool HasStartValue => _startValue.HasValue;
    public bool HasEndValue => _endValue.HasValue;
    public bool HasTwoValues => HasStartValue && HasEndValue;
    public int ValueCount => (HasStartValue ? 1 : 0) + (HasEndValue ? 1 : 0);
    //public int AbsDirection => HasTwoValues ? (EndValue > StartValue ? 1 : (EndValue < StartValue ? -1 : 0)) : 0;
    //public bool CanHaveDirection => HasTwoValues;
}

public enum Comparator
{
    Unknown,
    Known,
    LessThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
    GreaterThan
}