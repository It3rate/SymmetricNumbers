using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymNum.Primitives;
namespace SymNum.Expressions;

/// <summary>
/// IOrderable is anything that can be ordered, a measure, a number, expression, or pattern.
/// Expressions/bool operations and patterns do not have an obvious order, but the results do.
/// The order of the 16 bool expressions is therefore not universally set, but depends on the input.
/// It has variable certainty, and can participate in mathematical like operations.
/// </summary>
public interface IOrderable
{
	(float, float) ValueRange { get; }
	CertaintyLevel Certainty { get; }
	// IMeasures can combine with INumbers in limited ways, perhaps one it by replacing a specific slot (the op is the slot).
	IOrderable Combine(IOrderable other, BinaryOp op);

	// gt, lt, eq, sort, hash, 
}