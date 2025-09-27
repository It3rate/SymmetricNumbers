using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Patterns;

public class Mark
{
	public long Value { get; }
	public Comparator Direction { get; }
	public Mark(long value, Comparator direction = Comparator.Unknown)
	{
		Value = value;
		Direction = direction;
	}
	public Mark(long value)
	{
		Value = value;
		Direction = Comparator.Known;
	}
	public Mark(Comparator direction)
	{
		Value = 0;
		Direction = direction;
	}
	//public Extent StartToHere(Mark startPoint, Extent minmax)
	//{
	//	Extent result;
	//	if(Direction == Comparator.Known || startPoint.Direction == Comparator.Known)
	//	{
	//		result = minmax.Intersection(new Extent(startPoint.Value, Value));
	//	}
	//	//else if()

	//	return result;
	//}
	//public Extent HereToEnd(Mark endPoint, Extent minmax)
	//{

	//}
}
public enum Comparator
{
	Unknown,
	Known,
	LessThan,
	LessThanOrEqual,
	GreaterThanOrEqual,
	GreaterThan
}
