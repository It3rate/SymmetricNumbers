namespace Forces.Primitives.Electricity.Simulation.Circuits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RCL_TRAN_Circuit : CircuitBase
    {
        public RCL_TRAN_Circuit() : base(RCL_TRAN) { }


        public static string RCL_TRAN = """
* Series RLC Circuit Simulation
* Parameters: L=1mH, C=1uF, V=1V step
            
V1 in 0 PULSE(0 1 0 0.1ns 0.1ns 10ms 100ms)  ; Step from 0 to 1V at t=0
R1 in mid 10                           ; Resistance (vary for tests: 10, 63.25, 1000)
L1 mid out 1m                          ; Inductance
C1 out 0 1u                            ; Capacitance
            
.ic v(out)=0 i(L1)=0                   ; Initial conditions: vc=0, i=0
.tran 0.1us 5ms 0                      ; Transient analysis: 5ms sim time, 0.1us step
.save tran v(in) v(out) i(V1)         ; Print input voltage, capacitor voltage, current
.end
""";
    }
}
