using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Scraps;

// May not be needed, as the sequence of expression evaluation is probably internal, but may need to encode it specifically in some cases.

/// <summary>
/// A sequencer is really probably a type of number, which ultimately just describes some order and a range within it.
/// It is a description of rules for how properties can interact, but not commiting to any version of interaction.
/// 
/// It is separate class as it is related to time, and while that is essentially the ultimate ordered sequence,
/// the mechanics of series and parallel arrangements are quite specific in their behaviour and implications. 
/// It may or may not be worth having a distinct class in the end, but for now it makes this distinction.
/// A sequencer is like a number/equation -- a set of ordered values/operations in a broad sense.
/// Like types (a set of concepts and their relations), but stripped of their specific context. 
/// </summary>
public class Sequencer
{
    public Dictionary<long, List<Operator>> Sequence { get; private set; } = new Dictionary<long, List<Operator>>();
}
