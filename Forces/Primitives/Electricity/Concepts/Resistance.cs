namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;

public class Resistance : BinaryExpression
{
    public IValue Voltage => First;
    public IValue Current => Second;
    public override string Name => "Resistance";
    public Resistance(IValue voltage, IValue current) :
        base(voltage, current, TwoNumberOp.Divide, CertaintyLevel.Derived)
    { }
    public Resistance(SymmetricNumber voltageCurrent) : base(
        new ValueRef(voltageCurrent, RefKind.RealValue),
        new ValueRef(voltageCurrent, RefKind.ImaginaryValue), TwoNumberOp.Divide, CertaintyLevel.Derived)
    { }
}

