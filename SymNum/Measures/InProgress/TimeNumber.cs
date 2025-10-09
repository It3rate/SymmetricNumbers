using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.InProgress;

/// <summary>
/// Time has a special restriction in that it can only go forward and can't change its rate.
/// It is made of two extents, like rationals the denominator must be positive 
/// (aside: which is probably why we prefer rational to complex numbers?)
/// but the numerator can only represent 'now', and its change rate is fixed and unchangeable.
/// We can talk about time, about the past, about time travel, perception of time etc, 
/// but time itself is a very fixed position and change rate that applies to everything.
/// 
/// This is probably not worth having a specific number, mostly here to check,
/// and because 'virtual time' is heavily inolved in sequences, equations, causation etc.
/// Maybe time is the universal yardstick for order, making it the root of numbers rather than a type of number.
/// (probably this doesn't matter either way, but might be worth thinking about).
/// </summary>
public class TimeNumber
{
}
