using Forces.Primitives.Electricity.Simulation;
using Forces.Primitives.Electricity.Simulation.Circuits;

namespace Simulation
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

        private void btRunVIRm_Click(object sender, EventArgs e)
        {
            var circuit = new VIR_Man_Circuit();
            circuit.Run();
            tbOutput.Text = circuit.ResultText;
        }
        private void btRunRCLm_Click(object sender, EventArgs e)
        {
            var circuit = new RCL_Man_Circuit();
            circuit.Run();
            tbOutput.Text = circuit.ResultText;
        }
        private void btRunRCL_Click(object sender, EventArgs e)
        {
            var circuit = new RCL_AC_Circuit();
            circuit.Run();
            tbOutput.Text = circuit.ResultText;
        }
        private void btRunTransient_Click(object sender, EventArgs e)
        {
            var circuit = new RCL_TRAN_Circuit();
            circuit.Run();
            tbOutput.Text = circuit.ResultText;
        }
    }
}
