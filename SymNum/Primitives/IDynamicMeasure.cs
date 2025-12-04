using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Primitives;

public interface IDynamicMeasure<T> : IRefMeasure<T>
{
	long ProposedPosition { get; }
	/// <summary>
	/// Used to store the float remainder of a value. 
	/// Think a synapse's firing threshold as it's resolution, but charge below this resolution builds and decays.
	/// </summary>
	float Accumulator { get; }

	//float CalculateChange(Measure source);
	//void UpdateState(float change);

	public static Measure Min(Measure a, Measure b) => a.Position < b.Position ? a : b;
	public static Measure Max(Measure a, Measure b) => a.Position >= b.Position ? a : b;
	public static long MinPosition(Measure a, Measure b) => Math.Min(a.Position, b.Position);
	public static long MaxPosition(Measure a, Measure b) => Math.Max(a.Position, b.Position);
}
