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
	public IMeasure LeftEyeInput { get; }
	public UnbalancedNumber Sight { get; } 
	public BinaryExpression Thrust { get; }
	public float ThrustDirection { get; set; }
	public UnbalancedNumber Legs { get; }
	public DecayMeasure LeftEyeDecay;
	public DecayMeasure RightEyeDecay;
	public Creature()
	{ 
		RightEyeInput = new Measure(0);
		LeftEyeInput = new Measure(0);

		//var rightEyeMeasure = new MeasureRef(RightEyeInput, 5f);
		//var leftEyeMeasure = new MeasureRef(LeftEyeInput, 5f);
		//var RightEyeResolution = new DecayMeasure(RightEyeInput, 0.01f);
		//var LeftEyeResolution = new DecayMeasure(LeftEyeInput, 0.01f);
		LeftEyeDecay = new DecayMeasure(RightEyeInput, 0.4f);
		RightEyeDecay = new DecayMeasure(LeftEyeInput, 0.4f);

		Sight = new UnbalancedNumber(
			RightEyeDecay.PositionInLength,
			LeftEyeDecay.Length,
			RightEyeDecay.Length, 
			LeftEyeDecay.PositionInLength);

		// a ratio number has min and max DriftMeasrues, and creates a denominator with length,
		// and a measure with position - min. It supplies both numerator and denominator to half a number.

		//Thrust = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Add, null, CertaintyLevel.Derived);
		long min = 1;
		var LeftEyeToRightLeg = new DecayMeasure(LeftEyeInput, .01f);
		var RighttEyeToLeftLeg = new DecayMeasure(RightEyeInput, .01f);
		var  BrightnessRes = new Measure(255, CertaintyLevel.Set);
		Legs = new UnbalancedNumber(
			RighttEyeToLeftLeg, 
			BrightnessRes, 
			BrightnessRes,
			LeftEyeToRightLeg);
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
		//var left = Legs.LeftValue;
		//var right = Legs.RightValue;
		//var rot = 0f;
		//if (left != 0 && right != 0)
		//{
		//	rot = (left > right) ? -(left / right + .1f) : (right > left) ? (right / left + .1f) : 0;
		//}
		//ThrustDirection = (float)rot;
		ThrustDirection = Legs.CancelledSum(1);

		Trace.WriteLine(ThrustDirection.ToString("0.###"));
	}
    public void SetSight(long leftLight, long rightLight)
    {
        LeftEyeInput.SetPosition(leftLight);
        RightEyeInput.SetPosition(rightLight);
    }
}
