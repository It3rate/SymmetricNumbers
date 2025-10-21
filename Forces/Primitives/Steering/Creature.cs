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
	public MeasureMemory RightEyeMemory { get; }
    public Measure RightEye { get; } = new Measure(0, Direction.FacesRight);
    public Measure RightEyeResolution { get; } // creates a number, but also can be used to adjust sensitivity (reduce or increase the denominator realtive to the numerator)
    public MeasureMemory LeftEyeMemory { get; } // memory is used as the zero point
	public Measure LeftEye { get; } = new Measure(0, Direction.FacesLeft);
    public Measure LeftEyeResolution { get; } 
    public SplitNumber Sight { get; } // sight is constructed from the left and right eye, based around Sight zero according to above, regardless of the actual eye input values.

    public IValue LeftValue { get; }
	public IValue RightValue { get; }
	public IValue SightNeutral { get; }
	public IValue SightBrightness { get; }
	public BinaryExpression SightRatio { get; }
	public BinaryExpression SightLeftDif { get; }
	public BinaryExpression SightRightDif { get; }
	public BinaryExpression BrightnessDirection { get; }

	public IMeasure RightThrust { get; } = new Measure(10, Direction.FacesRight, true, CertaintyLevel.Unknown);
	public IMeasure LeftThrust { get; } = new Measure(30, Direction.FacesLeft, true, CertaintyLevel.Unknown);
	public IMeasure LeftThrustResolution { get; } = new Measure(_leftThrustResolution, Direction.FacesLeft, true, CertaintyLevel.Unknown);
	public IMeasure RightThrustResolution { get; } = new Measure(_rightThrustResolution, Direction.FacesRight, true, CertaintyLevel.Unknown);
	public SplitNumber Motion { get; }

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
		LeftEyeMemory = new MeasureMemory(LeftEye,  MeasureMemory.MemoryKind.Average, 1);
		RightEyeMemory = new MeasureMemory(RightEye, MeasureMemory.MemoryKind.Average, 1);
		Sight = new SplitNumber(RightEye, LeftEyeMemory, RightEyeMemory, LeftEye);

		LeftValue = Sight.CreateValueRef(RefKind.ImaginaryValue);
		RightValue = Sight.CreateValueRef(RefKind.RealValue);
		SightNeutral = Sight.CreateValueRef(RefKind.ValueAverage);
		SightBrightness = Sight.CreateValueRef(RefKind.UnitAverage);

		SightRatio = new BinaryExpression(LeftValue, RightValue, BinaryOp.Divide);
		SightLeftDif = new BinaryExpression(LeftValue, SightNeutral, BinaryOp.Divide);
		SightRightDif = new BinaryExpression(RightValue, SightNeutral, BinaryOp.Divide);
		BrightnessDirection = new BinaryExpression(SightLeftDif, SightRightDif, BinaryOp.Subtract);

		Motion = new SplitNumber(RightThrust, LeftThrustResolution, RightThrustResolution, LeftThrust);

        var leftThrustValue = Motion.CreateValueRef(RefKind.ImaginaryValue);
        var rightThrustValue = Motion.CreateValueRef(RefKind.RealValue);
		Thrust = new BinaryExpression(leftThrustValue, rightThrustValue, BinaryOp.Add);
		Turn = new BinaryExpression(leftThrustValue, rightThrustValue, BinaryOp.Subtract);
		TurnRatio = new BinaryExpression(Turn, Thrust, BinaryOp.Divide);
		// map the brightness diff to the default thrust (maybe based on darkenss) plus the turn signal.
		// the turn could just be the complex number diff, or multiply in each of the left and right ratios.
		TurnLeft = new BinaryExpression(SightLeftDif, leftThrustValue, BinaryOp.Add);
        TurnRight = new BinaryExpression(SightRightDif, rightThrustValue, BinaryOp.Add);
	}

    private Random _rnd = new Random();
	private long _leftLight = 2000;
	private long _rightLight = 2000;
	private long _leftInc = 100;
	public void OnTick()
    {

        // the resolution of a number will often not be set by the sensor itself. The light input is an eye thing, but the calibration 
        // is up to whatever uses that information. In this case, the leg thrust and the eye have to negotiate a resolution that powers the leg correctly.
        // The eye input value can be used by many other things, so there will be many numbers made with it, there is no single 'lumen' value everyone uses.
  //      var rightLight = _rnd.Next(2000) + 4000; // get from environment
		//var leftLight = _rnd.Next(2000) + 4000; // get from environment
		_leftLight += _leftInc;
		_rightLight += 20;
		// read light input
		RightEye.SetPosition(_rightLight);
		LeftEye.SetPosition(_leftLight);
		if(_leftLight > 5000)
		{
			_leftInc = -300;
		}
		else if(_leftLight < 600)
		{
			_leftInc = 300;
		}
        // this can be done with equations eventually (avg, val-avg, res)
        //Sight.Reset(LeftEyeMemory.Position, _leftLight, _leftThrustResolution, RightEyeMemory.Position, _rightLight, _rightThrustResolution);


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
    private void SetSight(long leftLight, long rightLight)
    {
        LeftEye.SetPosition(leftLight);
        RightEye.SetPosition(rightLight);
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
