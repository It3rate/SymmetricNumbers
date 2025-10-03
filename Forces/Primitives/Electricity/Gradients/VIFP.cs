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
    public class VIFP : SymmetricNumber
    {
        // "cycles per second" (Hz)     |  "joules per coulomb" (Volts)
        // ---------------------------  |  ----------------------------
        // "position in cycle" (phase)  |  "coulombs per second" (Amps) 
        public long Voltage => TopRight;
        public long Current => BottomRight;
        public long VPhaseShift => BottomLeft; // voltage leads (or follows) current 
        public long Frequency => TopLeft;

        public float ResistanceActual { get; set; } // measured
        public float VIResistance => RightValue; // Current == 0 ? (Voltage >= 0 ? long.MaxValue : long.MinValue) : Voltage / Current; 
        public float ResistanceDiff => ResistanceActual - VIResistance; // the difference is due to everything other than VI (eg. frequency, phase, noise, heat, etc)

        public float PowerActual { get; set; } // measured
        public float VIPower => Voltage * Current;
        public float PowerDiff => PowerActual - VIPower; // the difference is due to everything other than VI

        public float VPhaseActual { get; set; } // measured
        public float PFVPhase => Frequency == 0 ? 0 : (VPhaseShift % Frequency) / Frequency; // this is modulo in the real world, the number structure can (and should) reflect that.
        public float PhaseDiff => VPhaseActual - PFVPhase; // the difference is due to everything other than PF

        public VIFP(long voltage, long phase, long current, long frequency) : base(voltage, phase, current, frequency)
        {
            ResistanceActual = VIResistance;
        }
    }
}
