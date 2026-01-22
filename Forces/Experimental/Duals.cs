namespace Forces.Experimental.Duals;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Duals
{
    // This is a recursive system that always combines two objects of the same level. It normally creates a new level, but it might cast up, sideways or down.

    public interface Terminal
	{
		List<long> Measures { get; }
	}
    public interface ExprDAG
    {
        ExprDAG Left { get; }
        ExprDAG? Right { get; }
        Op Op { get; }
	}
    public interface Dual : ExprDAG
    {
        //Dual Left { get; } // these values may appear in different levels, and their components may also be reused
        //Dual Right { get; } // so any set value requires a possible negotiation (the center of an area wants to move right, but corner wants to move left.
        
        Certainty Certainty { get; set; } // anchoring energy
        long Repeats { get; } // this is the long value for the bottom level, or count for operations.
        float Eval { get; } // potentially yeild values
        List<Dual> Children { get; }
        TypeInfo TypeInfo { get; }

        Dual ProjectToSpace(Dual space);
    }
}
