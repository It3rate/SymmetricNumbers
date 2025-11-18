using Forces.Primitives.Electricity.Concepts;
using SymNum.Concepts;
using SymNum.Expressions;
using SymNum.Numbers;
using SymNum.Primitives;
using SymNum.Scraps;
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
	public IMeasure Tick { get; } = new Measure(0, CertaintyLevel.Set, false);

	// inputs
	public IMeasure MouthInput { get; }
	public IMeasure RightEyeInput { get; }
	public IMeasure LeftEyeInput { get; }

	// outputs
	public IMeasure RightLegOuput { get; }
	public IMeasure LeftLegOuput { get; }

	// stores
	public IMeasure EnergyStore { get; }
	public IMeasure LightMin { get; }
	public IMeasure LightMax { get; }
	public IMeasure LightSlowFollow { get; }
	public IMeasure LightFastFollow { get; }
	public IMeasure ThrustRejuvinationFill { get; }

	// synthesis
	public UnbalancedNumber Sight { get; }
	public Ratio Brightness { get; }
	public Ratio BrighterDirection { get; }

	// tuning
	public Ratio TurnScalar { get; }
	public Ratio ThrustScalar { get; }
	public Ratio EnergyLevelToThrustMaxScalar { get; }
	public Ratio SpeedToSatisfactionScale { get; }
	public Ratio FoodConversionScale { get; } // external, tuned to make sim run, not based on goals or calculations
	public Ratio MuscleBurnScale { get; } // external, tuned to make sim run, not based on goals or calculations
	public IMeasure AmbientEnergyUse { get; } // external, tuned to make sim run, not based on goals or calculations

	// goals
	public AnchorNumber EnergyGoal { get; }

	public BinaryExpression LightAndEnergyToThrust { get; set; }
	public Ratio ThrustDirection { get; set; }

	public DecayMeasure LeftEyeDecay;
	public DecayMeasure RightEyeDecay;

	public UnbalancedNumber LegThrust { get; }

	// strap: Mouth to energy : food (light areas) intake increase energy
	// strap: eyes to each leg : light direction to leg thrust difference
	// strap: eyes midpoint to legs : slow down when in desired area (to feed or hide)
	// strap: energy to legs : can't thrust as much when low energy
	// strap: energy to eyes-legs : low energy makes light seeking stronger
	// strap: each leg to energy : moving uses energy
	// strap: leg to leg : a leg thrust needs recovery time
	// strap: eye to eye : find day/night avg zero point, min max

	// EXPR: rightEye-leftEye=LightDir diff left and right eye : light show which direction is brighter
	// DECAY: average light over time : light level over time sets a zero point
	// FOLLOW: calibrate thrust to move the creature at the highest utility speed based on energy, light direction, environment

	// Mouth to energy : food (light areas) intake increase energy
	// EXPR : energy-cutoff=result // Energy level inverts light seeking behavior
	// EXPR : LightDir legs move toward or away from light
	// EXPR : legs move faster when not in state they want to be in
	// EXPR : legs move slower when tired

	// world adaptions: food goes down in spot when creature stays there too long
	// world adaptions: a high food area has more predators (perhaps just increase likelyhood of dying)
	// world adaptions: need to allow warping around edges of world sim, with paths and creatures.

	public Creature()
	{ 
		RightEyeInput = new Measure(0);
		LeftEyeInput = new Measure(0);
		LeftEyeDecay = new DecayMeasure(RightEyeInput, 0.08f);
		RightEyeDecay = new DecayMeasure(LeftEyeInput, 0.08f);


		var leftEyeRatio = new Ratio(LeftEyeDecay.PositionFromMinimum, LeftEyeDecay.Length, Polarity.FacesLeft);
		var rightEyeRatio = new Ratio(RightEyeDecay.PositionFromMinimum, RightEyeDecay.Length, Polarity.FacesRight);
		Sight = new UnbalancedNumber(leftEyeRatio, rightEyeRatio);

		// a ratio number has min and max DriftMeasures, and creates a denominator with length,
		// and a measure with position - min. It supplies both numerator and denominator to half a number.

		//Thrust = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Add, null, CertaintyLevel.Derived);
		var LeftEyeToRightLeg = new DecayMeasure(LeftEyeInput, 1f);
		var RighttEyeToLeftLeg = new DecayMeasure(RightEyeInput, 1f);
		var  BrightnessRes = new Measure(255, CertaintyLevel.Set);
		LegThrust = new UnbalancedNumber(
			RighttEyeToLeftLeg, 
			BrightnessRes, 
			BrightnessRes,
			LeftEyeToRightLeg);
		// expecting the result 0 will cause an error, which is the error diff, which is the query result (isTrue, isTouching, isOverlap, isLessThan etc)
		//TurnRatio = new BinaryExpression(Sight.GetImaginaryAsRational(), Sight.GetRational(), Ops.Subtract, RationalNumber.Zero, CertaintyLevel.Derived);
		ThrustDirection = new Ratio(0, 10000);
		EnergyStore = new Measure(2000, CertaintyLevel.Set, 0, 4000);
	}

	private Random _rnd = new Random();
	private long _leftLight = 0;
	private long _rightLight = 0;
	private long _leftInc = 4;
	public void OnTick()
	{
		Tick.Increment();
		// calculate internal reactions to input
		var left = Sight.LeftValue;
		var right = Sight.RightValue;
		var rot = 0f;
		if (left != 0 && right != 0)
		{
			rot = (left > right) ? -(left / right + .1f) : (right > left) ? (right / left + .1f) : 0;
		}
		ThrustDirection.Value = (float)rot;
		//ThrustDirection = Legs.CancelledSum(1);

		//Trace.WriteLine(ThrustDirection.ToString("0.###"));
	}
    public void SetInputs(long leftLight, long rightLight, long energyInput, long energyUse)
    {
		// Q. Should all updates happen on tick, or should they be independent of clock?
        LeftEyeInput.SetPositionAndInvoke(leftLight);
        RightEyeInput.SetPositionAndInvoke(rightLight);
		// Add energy based on how much light is there.
		EnergyStore.SetPositionAndInvoke(EnergyStore.Position + energyInput - energyUse);

		Trace.WriteLine(EnergyStore.Position);
	}
}
