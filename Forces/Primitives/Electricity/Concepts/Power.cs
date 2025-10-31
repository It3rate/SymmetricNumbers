namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Electricity.Momentum;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;

// Power is voltage * current, akin to area in length * width.
// 
public class Power : BinaryExpression
{
    public INumber Voltage => First;
    public INumber Current => Second;
    public override string Name => "Power";
    public Power(INumber voltage, INumber current) :
        base(voltage, current, Ops.Multiply, null, CertaintyLevel.Derived)
    { }
    public Power(SymmetricNumber voltageCurrent) :
        base(voltageCurrent.GetRational(), voltageCurrent.GetImaginary(), Ops.Multiply, null)
    { }
}
