using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Steering.Simulation;

public class Creature
{
	public Measure RightEye { get; } = new Measure(0, Direction.FacesRight);
	public MeasureMemory LeftEyeMemory { get; }
	public MeasureMemory RightEyeMemory { get; }
	public Measure LeftEye { get; } = new Measure(0, Direction.FacesLeft);
	public SymmetricNumber Sight { get; }

	public IValue LeftValue { get; }
	public IValue RightValue { get; }
	public IValue SightNeutral { get; }
	public IValue SightBrightness { get; }
	public BinaryExpression SightRatio { get; }
	public BinaryExpression SightLeftDif { get; }
	public BinaryExpression SightRightDif { get; }
	public BinaryExpression BrightnessDirection { get; }

	public IMeasure RightThrust { get; } = new Measure(0, Direction.FacesRight);
	public IMeasure LeftThrust { get; } = new Measure(0, Direction.FacesLeft);

	public Creature()
	{
		LeftEyeMemory = new MeasureMemory(LeftEye,  MeasureMemory.MemoryKind.Average);
		RightEyeMemory = new MeasureMemory(RightEye, MeasureMemory.MemoryKind.Average);
		Sight = new SymmetricNumber(RightEye, LeftEyeMemory, RightEyeMemory, LeftEye);

		LeftValue = Sight.CreateValueRef(RefKind.ImaginaryValue);
		RightValue = Sight.CreateValueRef(RefKind.RealValue);
		SightNeutral = Sight.CreateValueRef(RefKind.ValueAverage);
		SightBrightness = Sight.CreateValueRef(RefKind.UnitAverage);

		SightRatio = new BinaryExpression(LeftValue, RightValue, BinaryOp.Divide);
		SightLeftDif = new BinaryExpression(LeftValue, SightNeutral, BinaryOp.Divide);
		SightRightDif = new BinaryExpression(RightValue, SightNeutral, BinaryOp.Divide);
		BrightnessDirection = new BinaryExpression(SightLeftDif, SightRightDif, BinaryOp.Subtract);
	}
}
