namespace SymNum.Measures.InProgress;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;

/// <summary>
/// A number can be positive in either direction, and a complex/symmetric number can be both directions at once.
/// This requires the orientation to be made of both directions, which can be combined into a 360 degree style orientation.
/// If a line is grow to both the left and right, the direction of growth will be more than just a subtraction, it is akin to rotation.
/// </summary>
public class OrientationNumber : SplitNumber
{
    public OrientationNumber(IMeasure rightward, IMeasure leftward, IMeasure rightDegreeResolution, IMeasure leftDegreeResolution) 
        : base(rightward, leftward, rightDegreeResolution, leftDegreeResolution)
    {
    }
}
