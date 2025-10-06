namespace Forces.Primitives.Electricity.Gradients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SymNum.Patterns;

    /// <summary>
    /// Describes the voltage, current, frequency and phase shift of a measure. 
    /// The covers the basics of a sample, feels close to how a symmetric number could express it.
    /// This doesn't directly rely on units, as in, it could just measure uncalibrated values consistantly
    /// and have enough information to derive the math of circuits.
    /// Akin to how a microbe doesn't have lumens to calibrate against, or even a set point for neutral light that maps to both eyes.
    /// </summary>
    public class VIFΦ : SymmetricNumber
    {
        // "cycles per second" (Hz)     |  "joules per coulomb" (Volts)
        // ---------------------------  |  ----------------------------
        // "position in cycle" (phase)  |  "coulombs per second" (Amps) 
        public long Voltage => TopRight;
        public long Current => BottomRight;
        public long VPhaseShift => BottomLeft; // Φ, voltage leads (or follows) current 
		public long Frequency => TopLeft;

		// Other things with similar 4 value math (1 ratio and 1 repeat count/freq, 1 progress along repeats
        // (progress can be mod like in phase, or just the total distance travelled, like 7.2 times around vs 0.2 phase)
        // Loops: width(W), height(H), progress along diameter(P), repeat count(R) 
        // ForLoop: firstInstruction(F), lastInstruction(L), currentPosition(P), loopCount(C)
        // Populations: BirthRate(P), DeathRate(R), Time(T), TimePerGeneration(G)

		// Pure Resistance (not ractance) is V/I.
		// This is easy to calculate with V and I, but when setting resistance
		// it can be due to a change in voltage or current, caused by a change of component or input.
		// So changing the pure resistance is potentially from a change in voltage, current, or both.
		// Either one of these values can be ~anything, but the other has to be calculated off the resistance ratio.
		// If you set a resistance, and you know one of voltage, current, or power, you can fill in the others.
		// This means you can also 'fix' one and change a second and still know all the values.
		// The 'number' here is the relation of the values, it acts like a kind of 'unit' for the logic of the number.

		public float ResistanceActual { get; set; } // measured
		public float VIResistance => RightValue; // Current == 0 ? (Voltage >= 0 ? long.MaxValue : long.MinValue) : Voltage / Current; 
		public float ResistanceDiff => ResistanceActual - VIResistance; // the difference is due to everything other than VI (eg. frequency, phase, noise, heat, etc)


		// The idea here, is with a current and voltage time series, you can calculate a frequency. By checking which lags (V or I) you can get phase shift.
		// With the frequency you can calculate impedance (peakV/peakI)
		// With phase and impedance you can calculate capacitance and inductance.

		// This is a general pattern. You have a sensor, and with a time series you get information not available in the sensor. 
		// This new information is then treated like a new sensor input, and you can derive new information.
		// Eventually with enough probe/construct/change cycles, you can infer things in the world
		// (like the capacitance of a circuit from voltage/current measurements in a changing field, or even the component layout)

		// These samples are stored in Numbers, but they have no inherent zero point or unit size. Like the 'zero point' of a light sensor can 
		// be the average over recent samples, and the unit could be a normal amount of change that matters over time,
		// The current is like a unit for the voltage, and the frequency is the unit for the time series.
		// The numbers are values related to 'something' (the field, the opposite, anything else), and that something can be infered from a time series (equation).
		// Samples being able to generate their own higher level units allows the scaling up through levels - like voltage/current samples eventually sensing the code in a microchip.

		public float PhaseDegrees => ((VPhaseShift % Frequency) / (float)Frequency) * 90f; // Φ, from top of head, verify
		public float RMSVoltage => Voltage / SQRT2;
        public float RMSCurrent => Current / SQRT2;
        public float ImpedanceX => RMSVoltage / RMSCurrent;
        public float ResistanceR => ImpedanceX * MathF.Cos(PhaseDegrees * (MathF.PI / 180f));
        public float ReactanceX => ImpedanceX * MathF.Sin(PhaseDegrees * (MathF.PI / 180f));
        public float IndcutiveReactance => PhaseDegrees >= 0 ? ReactanceX : 0;
        public float CapacitiveReactance => PhaseDegrees < 0 ? ReactanceX : 0;

        public float Inductance => ReactanceX / (MathF.PI * 2f * Frequency); //XL/2πf



        public float PowerActual { get; set; } // measured
        public float VIPower => Voltage * Current;
        public float PowerDiff => PowerActual - VIPower; // the difference is due to everything other than VI

        public float VPhaseActual { get; set; } // Φ measured
		public float PFVPhase => Frequency == 0 ? 0 : (VPhaseShift % Frequency) / Frequency; // this is modulo in the real world, the number structure can (and should) reflect that.
        public float PhaseDiff => VPhaseActual - PFVPhase; // the difference is due to everything other than PF

        public VIFΦ(long voltage, long phase, long current, long frequency) : base(voltage, phase, current, frequency)
        {
            ResistanceActual = VIResistance;
        }

        public static float SQRT2 = (float)Math.Sqrt(2.0);
    }
}
