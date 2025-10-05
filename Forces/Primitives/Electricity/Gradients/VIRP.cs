using SymNum.Patterns;

namespace Forces.Primitives.Electricity.Gradients;

/// <summary>
/// Voltage, Current, Pure Resistance, and Power.
/// This is a number in the sense that any two values will set the other two.
/// So the 'unit' is dynamically calculated, and the scaling is not linear.
/// This might be the natural idea of symmetric numbers, where the numberline has two directions
/// but the location and size of the unit is not fixed.
/// In symmetry of 2D paths, the units are variable, but the meaning of each position is fixed 
/// but they are all independent of each other (there are no 'illegal moves' in motion).
/// At this level, the values are dynamic, but two of them must follow some equation the represents the 
/// relations of the values. This equation is 'intelligence' that the mini number system represents.
/// </summary>
public class VIRP : SymmetricNumber
{
	public long Voltage { get => TopRight; set => TopRight = value; }
	public long Current { get => BottomRight; set => BottomRight = value; }
	public long Power { get => BottomLeft; set => BottomLeft = value; }
	public long Resistance { get => TopLeft; set => TopLeft = value; }

	public VIRPState VoltageState { get; private set; }
	public VIRPState CurrentState { get; private set; }
	public VIRPState PowerState { get; private set; }
	public VIRPState ResistanceState { get; private set; }
	public override bool IsFullyDefined =>
		base.IsFullyDefined &&
		VoltageState == VIRPState.Set &&
		CurrentState == VIRPState.Set &&
		PowerState == VIRPState.Set &&
		ResistanceState == VIRPState.Set;

	public VIRP(long? voltage, long? current, long? power, long? resistance) :
		base(voltage ?? 0, current ?? 0, power ?? 0, resistance ?? 0)
	{
		VoltageState = voltage != null ? VIRPState.Set : VIRPState.Unknown;
		CurrentState = current != null ? VIRPState.Set : VIRPState.Unknown;
		PowerState = power != null ? VIRPState.Set : VIRPState.Unknown;
		ResistanceState = resistance != null ? VIRPState.Set : VIRPState.Unknown;
	}
	public void SetAndFix(VIRPKind toSet, long value, VIRPKind toFix)
	{
		FixValue(toFix);

	}
	public void SetTwo(VIRPKind toSet, long valueA, VIRPKind toFix, long valueB) 
	{
	}
	/// <summary>
	/// Use set and fixed/unknown to calculate all values, and then set them to 'set' when done.
	/// If not calculable then things turn to ranges with unknowns.
	/// </summary>
	private void Calculate()
	{
		// Calculate values based on what is set/fixed, and what is unknown/uncertain
		if(IsFullyDefined)
		{
			UnfixAll();
		}
	}

	public void SetValue(VIRPKind kind, long value)
	{
		UnfixAll();
		switch (kind)
		{
			case VIRPKind.Voltage: VoltageState = VIRPState.Fixed; break;
			case VIRPKind.Current: CurrentState = VIRPState.Fixed; break;
			case VIRPKind.Power: PowerState = VIRPState.Fixed; break;
			case VIRPKind.Resistance: ResistanceState = VIRPState.Fixed; break;
		}
	}
	public void FixValue(VIRPKind kind)
	{
		UnfixAll();
		switch (kind)
		{
			case VIRPKind.Voltage: VoltageState = VIRPState.Fixed; break;
			case VIRPKind.Current:	CurrentState = VIRPState.Fixed; break;
			case VIRPKind.Power: PowerState = VIRPState.Fixed; break;
			case VIRPKind.Resistance: ResistanceState = VIRPState.Fixed; break;
		}
	}
	public void UnfixAll()
	{
		VoltageState = VIRPState.Set;
		CurrentState = VIRPState.Set;
		PowerState = VIRPState.Set;
		ResistanceState = VIRPState.Set;
	}

	private void SetWithVI()
	{
		Resistance = (long)(Voltage / (float)Current);
		Power = Voltage * Current;
	}
	private void SetWithIR()
	{
		Voltage = Current * Resistance;
		Power = Current * Current * Resistance;
	}
	private void SetWithIP()
	{
		Voltage = (long)(Power / (double)Current);
		Resistance = (long)(Power / (double)(Current * Current));
	}
	private void SetWithVR()
	{
		Current = (long)(Voltage / (double)Resistance);
		Power = Voltage * Current;
	}
	private void SetWithVP()
	{
		Current = (long)(Power / (double)Voltage);
		Resistance = (long)(Power / ((double)Voltage * Voltage)); 
	}
	private void SetWithRP()
	{
		Current = (long)(Math.Sqrt(Power / (double)Resistance));
		Voltage = (long)(Math.Sqrt(Power * Resistance));
	}
}

public enum VIRPKind
{
	None,
	Voltage,
	Current,
	Power,
	Resistance,
}
public enum VIRPState
{
	Unknown,
	Uncertain,
	Set,
	Fixed,
}