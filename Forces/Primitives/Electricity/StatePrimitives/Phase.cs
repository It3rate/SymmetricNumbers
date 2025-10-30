namespace Forces.Primitives.Electricity.StatePrimitives;

using SymNum.Measures.Numbers;

public class Phase : ComplexNumber
{
    public Phase(long startPosition, long endPosition, long resolution) : base(startPosition, endPosition, resolution)
    {
    }
}
