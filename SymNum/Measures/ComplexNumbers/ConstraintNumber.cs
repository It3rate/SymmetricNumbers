using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.ComplexNumbers;
/// <summary>
/// These are relations across four measures that form a constraint.
/// Different values can be locked or relaxed while others are changed.
/// This allows the constraint to resolve using the uncertain values in order to resolidify the constraint.
/// If it is not solveable, a new constraint can be created that the solution must fall within.
/// E.g. Set a rect's area, you don't know if you need to change the width, height, or both.
/// but you do know the height for whatever the width is set to and vice versa.
/// This fact can be be expressed with a new constraint number, or derived from the area constraint (a=wh) in fact.
/// </summary>
public class ConstraintNumber
{
}
