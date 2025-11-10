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
    public IMeasure RightEyeInput { get; }	
	public IMeasure RightEyeResolution { get; }   
	public IMeasure LeftEyeInput { get; }   
	public IMeasure LeftEyeResolution { get; }
	public UnbalancedNumber Sight { get; } // sight is constructed from the left and right eye, based around Sight zero according to above, regardless of the actual eye input values.
	
	public IMeasure RightThrust { get; }	
	public IMeasure LeftThrust { get; }	
	public IMeasure LeftThrustResolution { get; }	
	public IMeasure RightThrustResolution { get; }
	public BinaryExpression Thrust { get; }
	
	public double BrighterDirection { get; set; }

	public UnbalancedNumber InvBrightness { get; }
	private Measure BrightnessRes = new Measure(100, CertaintyLevel.Set);

	public Creature()
	{ 
		RightEyeInput = new Measure(10);
		LeftEyeInput = new Measure(50);
		RightEyeResolution = new DecayMeasure(128, 1.1f, 1, 255, RightEyeInput);
		LeftEyeResolution  = new DecayMeasure(128, 1.1f, 1, 255, LeftEyeInput);

		//RightThrust = new Measure(10, true, CertaintyLevel.Uncertain);
		//LeftThrust = new Measure(10, true, CertaintyLevel.Uncertain);
		//LeftThrustResolution = new Measure(10, true, CertaintyLevel.Uncertain);
		//RightThrustResolution = new Measure(10, true, CertaintyLevel.Uncertain);

        Sight = new UnbalancedNumber(RightEyeInput, LeftEyeResolution, RightEyeResolution, LeftEyeInput);
		//Thrust = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Add, null, CertaintyLevel.Derived);
		long min = 1;
		var LeftEyeToRightLeg = new DecayMeasure(128, 10f, min, 255, LeftEyeInput);
		var RighttEyeToLeftLeg = new DecayMeasure(128, 10f, min, 255, RightEyeInput);
		InvBrightness = new UnbalancedNumber(BrightnessRes, RighttEyeToLeftLeg, LeftEyeToRightLeg, BrightnessRes);
		// expecting the result 0 will cause an error, which is the error diff, which is the query result (isTrue, isTouching, isOverlap, isLessThan etc)
		//TurnRatio = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Subtract, RationalNumber.Zero, CertaintyLevel.Derived);
		var measure = new Measure(100);
		var unit = new Measure(10);
		var test = new ComplexNumber(measure, measure, unit);
		var x = test.LeftValue;
	}

	private Random _rnd = new Random();
	private long _leftLight = 0;
	private long _rightLight = 0;
	private long _leftInc = 4;
	public void OnTick()
	{
		//Thrust.Calculate();
		var left = Sight.LeftValue;
		var right = Sight.RightValue;
		var rot = 0f;
		if (left != 0 && right != 0)
		{
			rot = (left > right) ? -(left / right + .1f) : (right > left) ? (right / left + .1f) : 0;
		}
		BrighterDirection = (float)rot;


		//BrighterDirection = MathF.Sqrt(Sight.BalancedValue(3));// * .002f;
		//if (double.IsInfinity(BrighterDirection) || double.IsNaN(BrighterDirection)){
		//	int x = 5;

		//	BrighterDirection = Sight.BalancedValue(1) * .2f;
		//}
		//Trace.WriteLine(BrighterDirection.ToString("0.###"));
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
