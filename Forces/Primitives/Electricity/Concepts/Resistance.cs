namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;

public class Resistance : BinaryExpression
{
    public INumber Voltage => First;
    public INumber Current => Second;
    public override string Name => "Resistance";
    public Resistance(INumber voltage, INumber current) :
        base(voltage, current, Ops.DivideTo, null, CertaintyLevel.Output)
    { }
    //public Resistance(SymmetricNumber voltageCurrent) :
    //    base( voltageCurrent.RightRatio(), voltageCurrent.LeftRation(), Ops.DivideTo, null)
    //{ }
}

