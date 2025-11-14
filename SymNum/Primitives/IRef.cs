namespace SymNum.Primitives;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRef : IMeasure
{
    IMeasure Ref { get; }
	//INumber Scale { get; }
	//INumber Bias { get; }
	
	void OnUpdate(IMeasure source); // this probably doesn't need the arg, should always be Ref?
}
