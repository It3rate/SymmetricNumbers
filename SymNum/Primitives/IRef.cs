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
	RationalNumber Scale { get; }
	long Bias { get; }
	void SetBias(long bias);
	//void OnUpdate(IMeasure source);
}
