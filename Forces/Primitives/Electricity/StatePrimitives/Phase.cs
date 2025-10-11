namespace Forces.Primitives.Electricity.StatePrimitives;

using SymNum.Measures.Numbers;

public class Phase : ComplexNumber
{
    public Phase(long resolution, long startPosition, long endPosition) : base(resolution, startPosition, endPosition)
    {
    }
}
