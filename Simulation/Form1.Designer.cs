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
            tbOutput = new TextBox();
            brRunRCL = new Button();
            SuspendLayout();
            // 
            // btRunVIR
            // 
            btRunVIR.Location = new Point(57, 67);
            btRunVIR.Margin = new Padding(4, 5, 4, 5);
            btRunVIR.Name = "btRunVIR";
            btRunVIR.Size = new Size(107, 38);
            btRunVIR.TabIndex = 0;
            btRunVIR.Text = "Run VIR";
            btRunVIR.UseVisualStyleBackColor = true;
            btRunVIR.Click += btRunVIR_Click;
            // 
            // tbOutput
            // 
            tbOutput.Location = new Point(187, 14);
            tbOutput.Margin = new Padding(4, 5, 4, 5);
            tbOutput.Multiline = true;
            tbOutput.Name = "tbOutput";
            tbOutput.Size = new Size(935, 1070);
            tbOutput.TabIndex = 1;
            // 
            // brRunRCL
            // 
            brRunRCL.Location = new Point(57, 138);
            brRunRCL.Margin = new Padding(4, 5, 4, 5);
            brRunRCL.Name = "brRunRCL";
            brRunRCL.Size = new Size(107, 38);
            brRunRCL.TabIndex = 0;
            brRunRCL.Text = "Run RCL";
            brRunRCL.UseVisualStyleBackColor = true;
            brRunRCL.Click += btRunRCL_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 1098);
            Controls.Add(tbOutput);
            Controls.Add(brRunRCL);
            Controls.Add(btRunVIR);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btRunVIR;
		private TextBox tbOutput;
        private Button brRunRCL;
    }
}
