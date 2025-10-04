namespace Forces.Primitives.Electricity.Simulation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SpiceSharpParser;

    public class SpiceParser
    {
        public static void DiodeCircuit()
        {
            var netlistText = string.Join(Environment.NewLine, "",
                    "Diode circuit",
                    "D1 OUT 0 1N914",
                    "V1 OUT 0 0",
                    ".model 1N914 D(Is=2.52e-9 Rs=0.568 N=1.752 Cjo=4e-12 M=0.4 tt=20e-9)",
                    ".DC V1 -1 1 10e-3",
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
            var export = spiceSharpModel.Exports.Find(e => e.Name == "i(V1)");
            simulation.EventExportData += (sender, args) => Console.WriteLine(export.Extract());
            var codes = simulation.Run(spiceSharpModel.Circuit, -1);
            codes = simulation.InvokeEvents(codes);
            codes.ToArray(); //eval
        }
    }
}
