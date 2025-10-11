namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forces.Primitives.Electricity.Momentum;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;

public class Power : BinaryExpression
{
    public IValue Voltage => First;
    public IValue Current => Second;
    public override string Name => "Power";
    public Power(IValue voltage, IValue current) :
        base(voltage, current, BinaryOp.Multiply, CertaintyLevel.Derived)
    { }
    public Power(SymmetricNumber voltageCurrent) : base(
        new ValueRef(voltageCurrent, RefKind.RealValue),
        new ValueRef(voltageCurrent, RefKind.ImaginaryValue), BinaryOp.Multiply, CertaintyLevel.Derived)
    { }
}
