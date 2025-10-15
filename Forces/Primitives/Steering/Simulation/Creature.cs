using Forces.Primitives.Electricity.Concepts;
using SymNum.Measures.Concepts;
using SymNum.Measures.Expressions;
using SymNum.Measures.Numbers;
using SymNum.Measures.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forces.Primitives.Steering.Simulation;

public class Creature : ConceptSpace
{
    // This eventually will be an learned concept space, where active elements are brought into focus based on context.
    // For now it is all manual to get the feel of it.
    // The whole creature is a concept space, but light is also a concept space, as is brightness.
    // Concepts are a blend of increasing abstraction starting from a number, to equations, to concepts, evironments, and ultimately worlds.
    public Measure RightEye { get; } = new Measure(0, Direction.FacesRight);
	public MeasureMemory LeftEyeMemory { get; }
	public MeasureMemory RightEyeMemory { get; }
	public Measure LeftEye { get; } = new Measure(0, Direction.FacesLeft);
	public SplitNumber Sight { get; }

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
	private static long _thrustResolution = 10;
	public IMeasure LeftThrustRes { get; } = new Measure(_thrustResolution, Direction.FacesLeft);
	public IMeasure RightThrustRes { get; } = new Measure(_thrustResolution, Direction.FacesRight);
	public SplitNumber Motion { get; }

	public BinaryExpression TurnLeft { get; }
	public BinaryExpression TurnRight { get; }

	public double Speed { get; set; } = 5;
	// this will be held in arena
	public double XLocation { get; set; }
	public double YLocation { get; set; }
	public double FacingDirection { get; set; }

	public Creature()
	{
		LeftEyeMemory = new MeasureMemory(LeftEye,  MeasureMemory.MemoryKind.Average);
		RightEyeMemory = new MeasureMemory(RightEye, MeasureMemory.MemoryKind.Average);
		Sight = new SplitNumber(RightEye, LeftEyeMemory, RightEyeMemory, LeftEye);

		LeftValue = Sight.CreateValueRef(RefKind.ImaginaryValue);
		RightValue = Sight.CreateValueRef(RefKind.RealValue);
		SightNeutral = Sight.CreateValueRef(RefKind.ValueAverage);
		SightBrightness = Sight.CreateValueRef(RefKind.UnitAverage);

		SightRatio = new BinaryExpression(LeftValue, RightValue, BinaryOp.Divide);
		SightLeftDif = new BinaryExpression(LeftValue, SightNeutral, BinaryOp.Divide);
		SightRightDif = new BinaryExpression(RightValue, SightNeutral, BinaryOp.Divide);
		BrightnessDirection = new BinaryExpression(SightLeftDif, SightRightDif, BinaryOp.Subtract);

		Motion = new SplitNumber(RightThrust, LeftThrustRes, RightThrustRes, LeftThrust);

		var leftThrustValue = Motion.CreateValueRef(RefKind.ImaginaryValue);
		var rightThrustValue = Motion.CreateValueRef(RefKind.RealValue);
		var motionTurn = new BinaryExpression(leftThrustValue, rightThrustValue, BinaryOp.Subtract);
		// map the brightness diff to the default thrust (maybe based on darkenss) plus the turn signal.
		// the turn could just be the complex number diff, or multiply in each of the left and right ratios.
		TurnLeft = new BinaryExpression(SightLeftDif, leftThrustValue, BinaryOp.Add);
		TurnRight = new BinaryExpression(SightRightDif, rightThrustValue, BinaryOp.Add);
	}
	public void OnTick()
	{
		// read light input
		var rightLight = 100; // get from environment
		RightEye.SetPosition(rightLight);
		var leftLight = 100; // get from environment
		LeftEye.SetPosition(leftLight);

		// set thrust
		Motion.SetValueFromRefKind(RefKind.RealValue, TurnRight.Value);
		Motion.SetValueFromRefKind(RefKind.ImaginaryValue, TurnLeft.Value);

		// move creature in environment based on thrust
		var rot = (TurnLeft.Value - TurnRight.Value) * 30; // max 60 degrees
		FacingDirection += rot;
		// add motion at constant speed in this Direction, wrap if at edge of image.
		// todo: motion and env read for creature.
		// This is probably the arena reading direction change and speed, creature doesn't know where it is.
	}
}
