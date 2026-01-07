namespace Forces.Experimental.Duals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Duals
{
    // This is a recursive system that always combines two objects of the same level. It normally creates a new level, but it might cast up, sideways or down.
    public interface Dual
    {
        Certainty Certainty { get; set; }
        Dual ChildLeft { get; }
        Dual ChildRight { get; }
        long Repeats { get; } // this is the long value for the bottom level.
        float Eval { get; }
        Op Op { get; }
        TypeInfo TypeInfo { get; }
    }
}
