using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Numbers
{
	/// <summary>
	/// A point number is the same measure with both perspectives, like a glass half empty or half full.
	/// The number 5 would be -5i+5, so it has a length of zero. 
	/// You can't use it as a rational number though, as (-3i+3)*(-2i+2) = -12i (not the point at 6).
	/// May or may not be useful.
	/// </summary>
	public class PointNumber : RawNumber
	{
		public PointNumber(long value, long resolution) :
			 this(new Measure(value), new Measure(resolution))
		{ }
		public PointNumber(float value, long resolution) : this((long)(value * resolution), resolution) { }
		public PointNumber(IMeasure measure, IMeasure unit)
		{
			RightMeasure = measure;
			RightUnit = unit;
			LeftUnit = unit;
			LeftMeasure = MeasureRef.MirroredRef(measure); // this locks 
		}
	}
}
