namespace SymNum.Mind;
public enum BoolState
{
    //FalseNegative,
    True,
    False,
    //TrueNegative,
    Underflow,
    Overflow,
    Unknown,
}
public static class BoolStateExtension
{
    public static bool BoolValue(this BoolState state) => state == BoolState.False ? false : true;
    public static bool IsTrue(this BoolState state) => state == BoolState.True;
    public static bool IsFalse(this BoolState state) => state == BoolState.False;
    public static bool IsBool(this BoolState state) => state == BoolState.False || state == BoolState.True;
    public static bool IsOutOfRange(this BoolState state) => state == BoolState.Underflow || state == BoolState.Overflow;
    public static bool AreBool(params BoolState[] states)
    {
        var result = true;
        foreach (var state in states)
        {
            if (!state.IsBool())
            {
                result = false;
                break;
            }
        }
        return result;
    }
    public static BoolState Invert(this BoolState state)
    {
        var result = BoolState.Unknown;
        switch (state)
        {
            case BoolState.False:
                result = BoolState.True;
                break;
            case BoolState.True:
                result = BoolState.False;
                break;
            case BoolState.Underflow:
                result = BoolState.Overflow;
                break;
            case BoolState.Overflow:
                result = BoolState.Underflow;
                break;
        }
        return result;
    }
}
