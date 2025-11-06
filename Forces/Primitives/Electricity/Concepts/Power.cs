namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Electricity.Momentum;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;

// Power is voltage * current, akin to area in length * width.
// 
public class Power : BinaryExpression
{
    public INumber Voltage => First;
    public INumber Current => Second;
    public override string Name => "Power";
    public Power(INumber voltage, INumber current) :
        base(voltage, current, Ops.MultiplyTo, null, CertaintyLevel.Derived)
    { }
    public Power(SymmetricNumber voltageCurrent) :
        base(voltageCurrent.GetRational(), voltageCurrent.GetImaginary(), Ops.MultiplyTo, null)
    { }
}
