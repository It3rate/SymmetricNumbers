using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Primitives;

public class MeasureMemory : MeasureBase
{
	public MemoryKind Kind { get; set; }
	public IMeasure Measure { get; }
	public override long Position => GetPostionByKind(Kind);
	public Queue<long> History { get; } = new Queue<long>();
	public int MaxHistory { get; }

	private bool _isFirst = true;

	public MeasureMemory(IMeasure measure, MemoryKind kind, int maxHistory = 5)
	{
		Measure = measure;
		Kind = kind;
		MaxHistory = maxHistory;
		Measure.PositionChanged += (pos) => SetPosition(pos);
	}
	public long GetPostionByKind(MemoryKind kind)
	{
		long result;
		switch (Kind)
		{
			case MemoryKind.Average:
				result = (long)MemoryAverage;
				break;
			case MemoryKind.Sum:
				result = (long)MemorySum;
				break;
			case MemoryKind.Min:
				result = (long)MemoryMin;
				break;
			case MemoryKind.Max:
				result = (long)MemoryMax;
				break;
			case MemoryKind.MaxDiff:
				result = (long)MemoryMaxDiff;
				break;
			case MemoryKind.Product:
				result = (long)MemoryProduct;
				break;

			case MemoryKind.None:
			default:
				result = base.Position;
				break;

		}
		return result;
	}
	public override void SetPosition(long value)
	{
		if (_isFirst)
		{
			for (int i = 0; i < MaxHistory; i++)
			{
				History.Enqueue(value);
			}
			_isFirst = false;
		}

		History.Enqueue(value);
		History.Dequeue();
	}

	public double MemoryAverage => History.Average();
	public double MemorySum => History.Sum();
	public double MemoryMin => History.Min();
	public double MemoryMax => History.Max();
	public double MemoryMaxDiff => MemoryMax - MemoryMin;
	public double MemoryProduct => History.Aggregate(1L, (a, b) => a * b);
	public enum MemoryKind
	{
		None,
		Average,
		Sum,
		Min,
		Max,
		MaxDiff,
		Product
	}
}
