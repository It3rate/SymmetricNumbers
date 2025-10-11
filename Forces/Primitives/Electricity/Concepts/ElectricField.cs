namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Measures.Concepts;
using SymNum.Measures.Expressions;

public class ElectricField : ConceptSpace
{
    public override string Name => "Electric Field";
    public IValue Voltage { get; }
    public IValue Current { get; }
    public Power Power { get; }
    public Resistance Resistance { get; }
    public ElectricField(IValue voltage, IValue current) 
    {
        Voltage = voltage;
        Current = current;
        Power = new Power(voltage, current);
        Resistance = new Resistance(voltage, current);

        ActiveConstraints.AddRange([Power, Resistance]);
    }
}
