namespace Simulation
{
	partial class Form1
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
            brRCL_AC = new Button();
            btRCL_Tran = new Button();
            tbOutput = new RichTextBox();
            btRclMan = new Button();
            SuspendLayout();
            // 
            // btRunVIR
            // 
            btRunVIR.Location = new Point(57, 70);
            btRunVIR.Margin = new Padding(4, 5, 4, 5);
            btRunVIR.Name = "btRunVIR";
            btRunVIR.Size = new Size(107, 38);
            btRunVIR.TabIndex = 0;
            btRunVIR.Text = "VIR man";
            btRunVIR.UseVisualStyleBackColor = true;
            btRunVIR.Click += btRunVIRm_Click;
            // 
            // brRCL_AC
            // 
            brRCL_AC.Location = new Point(57, 166);
            brRCL_AC.Margin = new Padding(4, 5, 4, 5);
            brRCL_AC.Name = "brRCL_AC";
            brRCL_AC.Size = new Size(107, 38);
            brRCL_AC.TabIndex = 0;
            brRCL_AC.Text = "RCL AC";
            brRCL_AC.UseVisualStyleBackColor = true;
            brRCL_AC.Click += btRunRCL_Click;
            // 
            // btRCL_Tran
            // 
            btRCL_Tran.Location = new Point(57, 214);
            btRCL_Tran.Margin = new Padding(4, 5, 4, 5);
            btRCL_Tran.Name = "btRCL_Tran";
            btRCL_Tran.Size = new Size(107, 38);
            btRCL_Tran.TabIndex = 0;
            btRCL_Tran.Text = "Transient";
            btRCL_Tran.UseVisualStyleBackColor = true;
            btRCL_Tran.Click += btRunTransient_Click;
            // 
            // tbOutput
            // 
            tbOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbOutput.Location = new Point(207, 12);
            tbOutput.Name = "tbOutput";
            tbOutput.Size = new Size(924, 1074);
            tbOutput.TabIndex = 2;
            tbOutput.Text = "Output";
            // 
            // btRclMan
            // 
            btRclMan.Location = new Point(57, 118);
            btRclMan.Margin = new Padding(4, 5, 4, 5);
            btRclMan.Name = "btRclMan";
            btRclMan.Size = new Size(107, 38);
            btRclMan.TabIndex = 0;
            btRclMan.Text = "RCL man";
            btRclMan.UseVisualStyleBackColor = true;
            btRclMan.Click += btRunRCLm_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 1098);
            Controls.Add(tbOutput);
            Controls.Add(btRCL_Tran);
            Controls.Add(brRCL_AC);
            Controls.Add(btRclMan);
            Controls.Add(btRunVIR);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button btRunVIR;
        private Button brRCL_AC;
        private Button btRCL_Tran;
        private RichTextBox tbOutput;
        private Button btRclMan;
    }
}
