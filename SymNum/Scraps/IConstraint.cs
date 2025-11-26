using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Scraps;

public interface IConstraint // Seems an IConstraint is nothing more than an IValue with undefined parts.
{
    //IValue First { get; }
    //IValue Second { get; }
    //TwoNumberOp Op { get; }
    //void SetCertaintyLevel(CertaintyLevel certaintyLevel);
    //IValue Calculate();
    string? Name { get; }
}
