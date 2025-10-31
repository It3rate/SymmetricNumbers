using SymNum.Numbers;
using SymNum.Scraps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymNum.Expressions;

// todo: equations are really just numbers with a more elaborate eval than n/1. They are inherent in numbers.

// spatially, you have 'here' (0 point), a value relative to here (1 there), a value relative to something (2 on), or relative to 2 things (3 between), or more (amongst).
// motion is similar, with a path defined.

// there is also a start/path/end dynamic, where we don't always specify all three. Eg: I'm here, I'm heading out, I'm heading home.
// words like after, along, farther, higher ,etc. don not specify a precise end point, so it is more start point, direction.
// towards, near, approaching don't have a clear start point, they are continuous and imprecise, to an end point.
// round and round, spinning, wandering don't have a start or end.
// here to there, a week, heaviest, have a start, path and end. (normal heavy to heaviest)

// we sometimes imply a unit with these segments, like twice as big, easier than X, even 'tiny house' is a full unit normal house compared to a full tiny house.
// that implies four samples.

// language also deals with repetition, with the same nuances.
// Again (was repeating for indeterminate time), daily (set times), over and over, last at bat (end point), flagging effort (rate of change),
// back and forth (binary repeat), spiraling out of control (scale change).

// motion can also have a function of change, steadily, quickly, accelerating, erratically, arc, flame out, slow burn, etc.

// all of these can be combined, like "quickly heading out to the big store, and back again".

// all of these are equations, and can be represented with numbers and operations (plus, minus, multiply, divide, repeat/exponent, state referential, dynamic follower etc)

public class Equation : RawNumber
{
    public Sequencer? Sequence { get; set; }
    public RepeatableNumber? RepetitionPattern { get; set; }
}
public enum Operations
{
    Add,
    Scale,
    Repeat,
    Sequence,
}