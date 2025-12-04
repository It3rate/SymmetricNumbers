namespace SymNum.Primitives;

using SymNum.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRefMeasure<T> : IMeasure<T>, IDisposable
{
	INumber NumberRef { get; }
	MeasureSlot Slot { get; }
	IMeasure<T> Ref { get; }

	CertaintyLevel Certainty { get; }
	bool IsInput { get; }
	Polarity Polarity { get; }
	IMeasure<T> Bias { get; }

	void OnExternalUpdate(IMeasure<long> source);

	T PositionAs(Polarity polarity);

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
