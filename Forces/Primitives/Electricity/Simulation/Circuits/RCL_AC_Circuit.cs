namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RCL_AC_Circuit : CircuitBase
    {
        public RCL_AC_Circuit() : base(RCL_AC) { }

        public static string RCL_AC = """
* Series RLC Circuit AC Simulation
* Parameters: L=1mH, C=1uF, Vin=1V AC

V1 in 0 AC 1                           ; 1V AC source
R1 in mid 10                           ; Resistance (vary for tests: 10, 63.25, 1000)
L1 mid out 1m                          ; Inductance
C1 out 0 1u                            ; Capacitance

.ac dec 50 1 1e6                       ; AC analysis: decade sweep, 50 pts/dec, 1 Hz to 1 MHz
.save ac v(in) v(out) i(V1)
* .save ac db(v(out)/v(in)) ph(v(out)/v(in)) ; Print magnitude (dB) and phase (deg) of transfer function
.end
""";

    }
}
