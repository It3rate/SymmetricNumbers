using Forces.Primitives.Electricity.Concepts;
using SymNum.Concepts;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public Measure RightEyeInput { get; } = new Measure(50, Direction.FacesRight);
    public DecayMeasure RightEyeResolution { get; } = new DecayMeasure(4, 64, 200, Direction.FacesRight);
    public Measure LeftEyeInput { get; } = new Measure(50, Direction.FacesLeft);
    public DecayMeasure LeftEyeResolution { get; } = new DecayMeasure(4, 64, 200, Direction.FacesLeft);
    public UnbalancedNumber Sight { get; } // sight is constructed from the left and right eye, based around Sight zero according to above, regardless of the actual eye input values.
	public BinaryExpression BrightnessDirection { get; }

	public IMeasure RightThrust { get; } = new Measure(10, Direction.FacesRight, true, CertaintyLevel.Uncertain);
	public IMeasure LeftThrust { get; } = new Measure(30, Direction.FacesLeft, true, CertaintyLevel.Uncertain);
	public IMeasure LeftThrustResolution { get; } = new Measure(_leftThrustResolution, Direction.FacesLeft, true, CertaintyLevel.Uncertain);
	public IMeasure RightThrustResolution { get; } = new Measure(_rightThrustResolution, Direction.FacesRight, true, CertaintyLevel.Uncertain);
	public UnbalancedNumber Motion { get; }

	public BinaryExpression Thrust { get; }
	public BinaryExpression TurnRatio { get; }
	public BinaryExpression Turn { get; }
	public BinaryExpression TurnLeft { get; }
	public BinaryExpression TurnRight { get; }

	public double Speed { get; set; } = 5;
	// this will be held in arena
	public double XLocation { get; set; }
	public double YLocation { get; set; }
	public double FacingDirection { get; set; }

	// these will be tuned values for sight and motion sensitivity
	private static long _leftSightResolution = 1000;
	private static long _rightSightResolution = 1000;
	private static long _leftThrustResolution = 10;
	private static long _rightThrustResolution = 10;

	public Creature()
    {
        RightEyeInput.PositionChanged += RightEyeResolution.OnTargetChange;
        LeftEyeInput.PositionChanged += LeftEyeResolution.OnTargetChange;
        Sight = new UnbalancedNumber(RightEyeInput, LeftEyeResolution, RightEyeResolution, LeftEyeInput);
		BrightnessDirection = new BinaryExpression(Sight.GetImaginary(), Sight.GetRational(), Ops.Subtract, null, CertaintyLevel.Derived);

		Motion = new UnbalancedNumber(RightThrust, LeftThrustResolution, RightThrustResolution, LeftThrust);

		//var leftThrustValue = Motion.CreateValueRef(RefKind.ImaginaryValue);
		//var rightThrustValue = Motion.CreateValueRef(RefKind.RealValue);
		Thrust = new BinaryExpression(Motion.GetImaginary(), Motion.GetRational(), Ops.Add, null, CertaintyLevel.Derived);
        // expecting the result 0 will cause an error, which is the error diff, which is the query result (isTrue, isTouching, isOverlap, isLessThan etc)
        TurnRatio = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Subtract, RationalNumber.Zero, CertaintyLevel.Derived);
    }

	private Random _rnd = new Random();
	private long _leftLight = 0;
	private long _rightLight = 0;
	private long _leftInc = 4;
	public void OnTick()
	{
		TurnRatio.Calculate();
		var rot = (TurnRatio.Result.GetImaginary().FloatValue - TurnRatio.Result.GetRational().FloatValue); // max 60 degrees
		FacingDirection = (float)Math.Clamp(rot, -3f, 3f); 
		//Trace.WriteLine(rot);
        // add motion at constant speed in this Direction, wrap if at edge of image.
        // todo: motion and env read for creature.
        // This is probably the arena reading direction change and speed, creature doesn't know where it is.
    }
    public void SetSight(long leftLight, long rightLight)
    {
        LeftEyeInput.SetPosition(leftLight);
        RightEyeInput.SetPosition(rightLight);
    }
    private void SetSightDenominators(long leftThrustRes, long rightThrustRes)
    {
        LeftEyeResolution.SetPosition(leftThrustRes);
        RightEyeResolution.SetPosition(rightThrustRes);
    }
    private void SetMotion(long leftThrust, long rightThrust)
    {
        LeftThrust.SetPosition(leftThrust);
        RightThrust.SetPosition(rightThrust);
    }
    private void SetMotionDenominators(long leftThrustRes, long rightThrustRes)
    {
        LeftThrustResolution.SetPosition(leftThrustRes);
        RightThrustResolution.SetPosition(rightThrustRes);
    }
}
