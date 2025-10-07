namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Forces.Primitives.Electricity.Gradients;
    using SpiceSharpParser.Common;
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters.VoltageExports;

    public class RCL_AC_Circuit : CircuitBase
    {
        private List<VIFΦ> _samples = new List<VIFΦ>();
        private StringBuilder _sb = new StringBuilder();
        public RCL_AC_Circuit() : base(RCL_AC)
        {
            SampleReceived += RCL_AC_Circuit_SampleReceived;
        }

        private void RCL_AC_Circuit_SampleReceived(object? sender, Export[] e)
        {
            if (CurrentSim != null && CurrentSim is ACWithEvents sim)
            {
                var freq = sim.Frequency;
                var v_inVal = e[0].Extract();
                var v_midVal = e[1].Extract();
                var vr_inVal = e[2].Extract();
                var vi_inVal = e[3].Extract();
                var vr_outVal = e[4].Extract();
                var vi_outVal = e[5].Extract();
                var vp_outVal = e[6].Extract();
                var vdb_outVal = e[7].Extract();
                var iv1_val = e[8].Extract();
                var sample = new VIFΦ((long)v_midVal, (long)vp_outVal, (long)iv1_val, (long)freq);
                _sb.AppendLine($"V(mid)={v_midVal:F4} \tI(V1)={iv1_val * 1000:F8} \tvPhase={vp_outVal:F6} \tFreq={freq:F0}");
            }
        }
        public override string ResultText => _sb.ToString();

        // "i", "ir", "ii", "im", "idb", "ip"
        // cur, real, imag, mag, decibel, phase
        //"v", "vr", "vi", "vm", "vdb", "vp", "vph" 
        // v, vreal, vimag, VoltageMagnitude, VoltageDecibel, VoltagePhase
        public static string RCL_AC = """
* Series RLC Circuit AC Simulation
* Parameters: L=1mH, C=1uF, Vin=1V AC

V1 in 0 AC 1                           ; 1V AC source
R1 in mid 10                           ; Resistance (vary for tests: 10, 63.25, 1000)
L1 mid out 1m                          ; Inductance
C1 out 0 1u                            ; Capacitance

.ac dec 50 1 1e6                       ; AC analysis: decade sweep, 50 pts/dec, 1 Hz to 1 MHz

.save ac v(in) v(mid) vr(in) vi(in) vr(out) vi(out) vp(out) vdb(out) i(V1)

* .save ac db(v(out)/v(in)) ph(v(out)/v(in)) ; Print magnitude (dB) and phase (deg) of transfer function
.end
""";

    }
}
