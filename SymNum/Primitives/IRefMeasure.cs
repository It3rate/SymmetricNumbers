namespace SymNum.Primitives;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRefMeasure : IMeasure, IDisposable
{
	INumber NumberRef { get; }
	MeasureSlot Slot { get; }
    IMeasure Ref { get; }

	CertaintyLevel Certainty { get; }
	bool IsCertain { get; }
	Polarity Polarity { get; }
	IMeasure Bias { get; }

	void OnExternalUpdate(IMeasure source);

	// unary ops
	//public void Increment();
	//public void Decrement();
	//void Negate();
	//long Half();
	//long Double();

	// no range limits for measures
	//long Minimum { get; set; } // does measure have a min max?
	//long Maximum { get; set; }
	//public bool CanBeNegative { get; set; }
	//bool IsFullDefined => !IsMin && !IsMax;
	//bool IsMax => Position == long.MaxValue;
	//bool IsMin => Position == long.MinValue;
	//bool IsZero => Position == 0 && IsFullDefined;
}
