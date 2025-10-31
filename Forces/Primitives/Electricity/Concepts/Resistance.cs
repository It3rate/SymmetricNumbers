namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;

public class Resistance : BinaryExpression
{
    public INumber Voltage => First;
    public INumber Current => Second;
    public override string Name => "Resistance";
    public Resistance(INumber voltage, INumber current) :
        base(voltage, current, Ops.Divide, null, CertaintyLevel.Derived)
    { }
    public Resistance(SymmetricNumber voltageCurrent) :
        base( voltageCurrent.GetRational(), voltageCurrent.GetImaginary(), Ops.Divide, null)
    { }
}

