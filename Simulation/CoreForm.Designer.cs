namespace Simulation
{
	partial class CoreForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			btRunVIR = new Button();
			btRCL_AC = new Button();
			btRCL_Tran = new Button();
			tbOutput = new RichTextBox();
			btRclMan = new Button();
			btNext = new Button();
			corePanel = new Panel();
			SuspendLayout();
			// 
			// btRunVIR
			// 
			btRunVIR.Location = new Point(12, 17);
			btRunVIR.Name = "btRunVIR";
			btRunVIR.Size = new Size(75, 23);
			btRunVIR.TabIndex = 0;
			btRunVIR.Text = "VIR man";
			btRunVIR.UseVisualStyleBackColor = true;
			btRunVIR.Click += btRunVIRm_Click;
			// 
			// btRCL_AC
			// 
			btRCL_AC.Location = new Point(12, 75);
			btRCL_AC.Name = "btRCL_AC";
			btRCL_AC.Size = new Size(75, 23);
			btRCL_AC.TabIndex = 0;
			btRCL_AC.Text = "RCL AC";
			btRCL_AC.UseVisualStyleBackColor = true;
			btRCL_AC.Click += btRunRCL_Click;
			// 
			// btRCL_Tran
			// 
			btRCL_Tran.Location = new Point(12, 103);
			btRCL_Tran.Name = "btRCL_Tran";
			btRCL_Tran.Size = new Size(75, 23);
			btRCL_Tran.TabIndex = 0;
			btRCL_Tran.Text = "Transient";
			btRCL_Tran.UseVisualStyleBackColor = true;
			btRCL_Tran.Click += btRunTransient_Click;
			// 
			// tbOutput
			// 
			tbOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tbOutput.Location = new Point(145, 7);
			tbOutput.Margin = new Padding(2);
			tbOutput.Name = "tbOutput";
			tbOutput.Size = new Size(648, 646);
			tbOutput.TabIndex = 2;
			tbOutput.Text = "Output";
			// 
			// btRclMan
			// 
			btRclMan.Location = new Point(12, 46);
			btRclMan.Name = "btRclMan";
			btRclMan.Size = new Size(75, 23);
			btRclMan.TabIndex = 0;
			btRclMan.Text = "RCL man";
			btRclMan.UseVisualStyleBackColor = true;
			btRclMan.Click += btRunRCLm_Click;
			// 
			// btNext
			// 
			btNext.Location = new Point(12, 602);
			btNext.Name = "btNext";
			btNext.Size = new Size(75, 23);
			btNext.TabIndex = 3;
			btNext.Text = "Next";
			btNext.UseVisualStyleBackColor = true;
			btNext.Click += NextPanel;
			// 
			// corePanel
			// 
			corePanel.Location = new Point(122, 7);
			corePanel.Name = "corePanel";
			corePanel.Size = new Size(620, 620);
			corePanel.TabIndex = 4;
			corePanel.Visible = false;
			// 
			// CoreForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 637);
			Controls.Add(corePanel);
			Controls.Add(btNext);
			Controls.Add(tbOutput);
			Controls.Add(btRCL_Tran);
			Controls.Add(btRCL_AC);
			Controls.Add(btRclMan);
			Controls.Add(btRunVIR);
			Name = "CoreForm";
			Text = "Form1";
			ResumeLayout(false);
		}

		#endregion

		private Button btRunVIR;
        private Button btRCL_AC;
        private Button btRCL_Tran;
        private RichTextBox tbOutput;
        private Button btRclMan;
		private Button btNext;
		private Panel corePanel;
	}
}
