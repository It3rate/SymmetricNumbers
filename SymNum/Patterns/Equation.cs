using SymNum.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

public class Equation : NumberBase
{
    public Sequencer Sequence { get; set; }
    public RepeatableNumber RepetitionPattern { get; set; }
}
public enum Operations
{
    Add,
    Scale,
    Repeat,
    Sequence,
}