namespace Forces.Primitives.Electricity.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices.JavaScript;
    using System.Text;
    using System.Threading.Tasks;
    using SpiceSharp;
    using SpiceSharp.Simulations;
    using SpiceSharpParser;
    using SpiceSharpParser.ModelReaders.Netlist.Spice;
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;

    public class SpiceParser
    {
        public static (SpiceSharpModel, Export[]) RCLCircuit()
        {
            var netlistText = """
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

            // Parsing part
            var parser = new SpiceNetlistParser();
            var parseResult = parser.ParseNetlist(netlistText);
            var netlist = parseResult.FinalModel;

            // Translating netlist model to SpiceSharp
            var reader = new SpiceSharpReader();
            var spiceSharpModel = reader.Read(netlist);

            // Simulation using SpiceSharp
            var simulation = spiceSharpModel.Simulations.Single();
            Export? export = spiceSharpModel.Exports.Find(e => e.Name == "i(V1)");

            return (spiceSharpModel, [export!]) ;

        }
        public static (SpiceSharpModel, Export[]) DiodeCircuit()
        {
            var netlistText = string.Join(Environment.NewLine, "",
                    "Diode circuit",
                    "D1 OUT 0 1N914",
                    "V1 OUT 0 0",
                    ".model 1N914 D(Is=2.52e-9 Rs=0.568 N=1.752 Cjo=4e-12 M=0.4 tt=20e-9)",
                    ".DC V1 -1 1 0.05",
                    ".SAVE i(V1)",
                    ".END");

            // Parsing part
            var parser = new SpiceNetlistParser();
            var parseResult = parser.ParseNetlist(netlistText);
            var netlist = parseResult.FinalModel;

            // Translating netlist model to SpiceSharp
            var reader = new SpiceSharpReader();
            var spiceSharpModel = reader.Read(netlist);

            // Simulation using SpiceSharp
            var simulation = spiceSharpModel.Simulations.Single();
            Export? export = spiceSharpModel.Exports.Find(e => e.Name == "i(V1)");

            return (spiceSharpModel, [export!]) ;

        }

        public static List<double[]> Run(SpiceSharpModel model, params Export[] exports)
        {
            var result = new List<double[]>();
            var sim = model.Simulations.Single();
            foreach (var export in exports)
            {
                sim.EventExportData += (sender, args) => result.Add([export.Extract()]);
            }
            var codes = sim.Run(model.Circuit, -1);
            codes = sim.InvokeEvents(codes);
            codes.ToArray(); //eval
            return result;
        }
        public static string FormatToColumns(List<double[]> data, string[] headers = null)
        {
            if (data.Count == 0) return "";

            int cols = data[0].Length;

            List<string[]> strData = data.Select(row => row.Select(d => d.ToString()).ToArray()).ToList();

            if (headers != null && headers.Length == cols)
            {
                strData.Insert(0, headers);
            }

            int[] widths = new int[cols];
            for (int i = 0; i < cols; i++)
            {
                widths[i] = strData.Max(row => row[i].Length);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var row in strData)
            {
                for (int i = 0; i < cols; i++)
                {
                    sb.Append(row[i].PadRight(widths[i] + 2)); // +2 for spacing between columns
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
