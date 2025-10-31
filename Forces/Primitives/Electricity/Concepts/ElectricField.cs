namespace Forces.Primitives.Electricity.Concepts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Concepts;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;

public class ElectricField : ConceptSpace
{
    public override string Name => "Electric Field";
    public INumber Voltage { get; }
    public INumber Current { get; }
    public Power Power { get; }
    public Resistance Resistance { get; }
    public ElectricField(INumber voltage, INumber current) 
    {
        Voltage = voltage;
        Current = current;
        Power = new Power(voltage, current);
        Resistance = new Resistance(voltage, current);

        ActiveConstraints.AddRange([Power, Resistance]);
    }
}
