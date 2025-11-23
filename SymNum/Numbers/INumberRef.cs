using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers;

public interface INumberRef : INumber
{
	INumber Ref { get; }
	INumber Scale { get; }
	IMeasure Bias { get; }
}
