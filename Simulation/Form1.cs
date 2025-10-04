using Forces.Primitives.Electricity.Simulation;

namespace Simulation
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

        private void btRunVIR_Click(object sender, EventArgs e)
        {
            var circuit = new SpiceCircuit();
            circuit.RunCircuitVIR();
            tbOutput.Text = circuit.GetOutput();
        }
        private void btRunRCL_Click(object sender, EventArgs e)
        {
            var circuit = new SpiceCircuit();
            circuit.RunCircuitRCL();
            tbOutput.Text = circuit.GetOutput();
        }
    }
}
