using SymNum.Measures.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymNum.Measures.Expressions;

public interface IValue
{
	// The set part of this can't always set it's own value.
	// For example in a BinaryExpression it examines which inputs are fixed and which are variable,
	// and adjusts them as needed.
	public double Value { get; }
	public CertaintyLevel CertaintyLevel { get; }
}
public class EmptyValue : IValue
{
	private static EmptyValue _instance = new EmptyValue();
	public static EmptyValue Instance => _instance;
	public CertaintyLevel CertaintyLevel => CertaintyLevel.Fixed;
	public double Value { get => 0; }
}