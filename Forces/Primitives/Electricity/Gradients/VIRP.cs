using Forces.Primitives.Sound.StatePrimitives;
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
    // Other things with similar 4 value math (1 ratio and 1 product):
    // (the meta pattern is something like a thing divisible into two active components where the ratio matters)
    // The idea is that this pattern is encoded/discovered as a number (four measures) with these constraints. Then similar relations can reuse the pattern.

    // Thermodynamics: temp(T), heat capacity(C), heat energy(Q), entropy(S) Q=CT and S=Q/T
    // Optics: wavelength(λ), freqency(f), speed of light(C), energy(E) C=λf and E=ht (h is planck's const)
    // Economics: price(P), demand(D), supply(S), revenue(R) R=PD and P=S/D for equilibrium
    // Data Transmission: bandwidth(B), sig2noise(SNR), data rate(R), symbol rate(S) R=Blog2(1+SNR) and S~RB
    // Chemistry: concentration(C), molar mass(M), volume(V), mass(m = nM) C and V fix n and m with div and mult
    // Biology; birth rate(B) death rate(D), population(N), net growth(G) G=N(B-D) and pop change relates to B=G/N+D
    // Mechanics: force(F), velocity(V), mass(M), kinetic energy(K) K=1/2MV^2 and F=dK/dx
    // Social groups: status(S), interaction freq(F), connection strength(C), group cohesion(G) G=CF, C=G/S
    // relations: encounters(e), value(v), strength(s), utility(u) s=ev, u=v/e
    // Love: time(t), attraction(A), intimacy(I), commitment(C) C=I/t, A=I/t^2

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
	public int SetSlotsValue =>
		VoltageState.SlotValue(VIRPKind.Voltage) +
		CurrentState.SlotValue(VIRPKind.Current) +
		PowerState.SlotValue(VIRPKind.Power) +
		ResistanceState.SlotValue(VIRPKind.Resistance);

	public VIRP(long? voltage, long? current, long? power, long? resistance) :
		base(voltage ?? 0, current ?? 0, power ?? 0, resistance ?? 0)
	{
		VoltageState = voltage != null ? VIRPState.Set : VIRPState.Unknown;
		CurrentState = current != null ? VIRPState.Set : VIRPState.Unknown;
		PowerState = power != null ? VIRPState.Set : VIRPState.Unknown;
		ResistanceState = resistance != null ? VIRPState.Set : VIRPState.Unknown;
	}
	public VIRPState GetState(VIRPKind kind)
	{
		var result = VIRPState.Unknown;
		switch (kind)
		{
			case VIRPKind.Voltage:
				result = VoltageState;
				break;
			case VIRPKind.Current:
				result = CurrentState;
				break;
			case VIRPKind.Power:
				result = PowerState;
				break;
			case VIRPKind.Resistance:
				result = ResistanceState;
				break;
		}
		return result;
	}
	public long GetValue(VIRPKind kind)
	{
		long result = 0; // probably should be max or min based on direction?
		if (GetState(kind).HasValue())
		{
			switch (kind)
			{
				case VIRPKind.Voltage:
					result = Voltage;
					break;
				case VIRPKind.Current:
					result = Current;
					break;
				case VIRPKind.Power:
					result = Power;
					break;
				case VIRPKind.Resistance:
					result = Resistance;
					break;
			}
		}
		return result;
	}
	public void FixValue(VIRPKind kind)
	{
		switch (kind)
		{
			case VIRPKind.Voltage: VoltageState = VIRPState.Fixed; break;
			case VIRPKind.Current: CurrentState = VIRPState.Fixed; break;
			case VIRPKind.Power: PowerState = VIRPState.Fixed; break;
			case VIRPKind.Resistance: ResistanceState = VIRPState.Fixed; break;
		}
	}
	public void SetValue(VIRPKind kind, long value)
	{
		switch (kind)
		{
			case VIRPKind.Voltage:
				Voltage = value;
				VoltageState = VIRPState.Set;
				break;
			case VIRPKind.Current:
				Current = value;
				CurrentState = VIRPState.Set;
				break;
			case VIRPKind.Power:
				Power = value;
				PowerState = VIRPState.Set;
				break;
			case VIRPKind.Resistance:
				Resistance = value;
				ResistanceState = VIRPState.Set;
				break;
		}
	}
	public void SetAndFix(VIRPKind toSet, long value, VIRPKind toFix)
	{
		UnfixAll();
		FixValue(toFix);
		SetValue(toSet, value);
		Calculate();
	}
	public void SetTwo(VIRPKind setA, long valueA, VIRPKind setB, long valueB)
	{
		UnfixAll();
		SetValue(setA, valueA);
		SetValue(setB, valueB);
		Calculate();
	}
	/// <summary>
	/// Use set and fixed/unknown to calculate all values, and then set them to 'set' when done.
	/// If not calculable then things turn to ranges with unknowns.
	/// </summary>
	private void Calculate()
	{
		// Calculate values based on what is set/fixed, and what is unknown/uncertain
		var ssv = SetSlotsValue;
		var canFullyCalc = true;
		//Voltage = 1,
		//Current = 2,
		//Power = 4,
		//Resistance = 8,
		switch (ssv)
		{
			case 3: // VI
				SetWithVI();
				break;
			case 5: // VP
				SetWithVP();
				break;
			case 6: // IP
				SetWithIP();
				break;
			case 7: // VIP
				SetWithVIP();
				break;
			case 8: // VR
				SetWithVR();
				break;
			case 10: // IR
				SetWithIR();
				break;
			case 11: // VIR
				SetWithVIR();
				break;
			case 12: // PR
				SetWithPR();
				break;
			case 13: // VPR
				SetWithVPR();
				break;
			case 14: // IPR
				SetWithIPR();
				break;
			default:
				canFullyCalc = false;
				break;
		}
		if(canFullyCalc)
		{
			UnfixAll();
		}
	}

	public void UnfixAll()
	{
		VoltageState = VIRPState.Set;
		CurrentState = VIRPState.Set;
		PowerState = VIRPState.Set;
		ResistanceState = VIRPState.Set;
	}

	private void SetWithVIR()
	{
		Power = Voltage * Current;
	}
	private void SetWithIPR()
	{
		Voltage = Current * Resistance;
	}
	private void SetWithVPR()
	{
		Current = (long)(Voltage / (double)Resistance);
	}
	private void SetWithVIP()
	{
		Power = Voltage * Current;
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
	private void SetWithPR()
	{
		Current = (long)(Math.Sqrt(Power / (double)Resistance));
		Voltage = (long)(Math.Sqrt(Power * Resistance));
	}
}

public enum VIRPKind
{
	None = 0,
	Voltage = 1,
	Current = 2,
	Power = 4,
	Resistance = 8,
}
public static class VIRPKindExtensions
{ 
	public static int SlotValue(this VIRPKind kind, VIRPState state)
	{
		return state.HasValue() ? (int)kind : 0;
	}
}
public enum VIRPState
{
	Unknown,
	Uncertain,
	Set,
	Fixed,
}

public static class VIRPStateExtensions
{
	public static bool HasValue(this VIRPState state) => state == VIRPState.Fixed || state == VIRPState.Set;
	public static int SlotValue(this VIRPState state, VIRPKind kind)
	{
		return state.HasValue() ? (int)kind : 0;
	}
}