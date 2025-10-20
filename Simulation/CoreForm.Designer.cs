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
            btRunVIR.Location = new Point(57, 70);
            btRunVIR.Margin = new Padding(4, 5, 4, 5);
            btRunVIR.Name = "btRunVIR";
            btRunVIR.Size = new Size(107, 38);
            btRunVIR.TabIndex = 0;
            btRunVIR.Text = "VIR man";
            btRunVIR.UseVisualStyleBackColor = true;
            btRunVIR.Click += btRunVIRm_Click;
            // 
            // btRCL_AC
            // 
            btRCL_AC.Location = new Point(57, 167);
            btRCL_AC.Margin = new Padding(4, 5, 4, 5);
            btRCL_AC.Name = "btRCL_AC";
            btRCL_AC.Size = new Size(107, 38);
            btRCL_AC.TabIndex = 0;
            btRCL_AC.Text = "RCL AC";
            btRCL_AC.UseVisualStyleBackColor = true;
            btRCL_AC.Click += btRunRCL_Click;
            // 
            // btRCL_Tran
            // 
            btRCL_Tran.Location = new Point(57, 213);
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
            // btNext
            // 
            btNext.Location = new Point(57, 1003);
            btNext.Margin = new Padding(4, 5, 4, 5);
            btNext.Name = "btNext";
            btNext.Size = new Size(107, 38);
            btNext.TabIndex = 3;
            btNext.Text = "Next";
            btNext.UseVisualStyleBackColor = true;
            btNext.Click += NextPanel;
            // 
            // corePanel
            // 
            corePanel.Location = new Point(209, 12);
            corePanel.Margin = new Padding(4, 5, 4, 5);
            corePanel.Name = "corePanel";
            corePanel.Size = new Size(920, 1030);
            corePanel.TabIndex = 4;
            corePanel.Visible = false;
            // 
            // CoreForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 1062);
            Controls.Add(corePanel);
            Controls.Add(btNext);
            Controls.Add(tbOutput);
            Controls.Add(btRCL_Tran);
            Controls.Add(btRCL_AC);
            Controls.Add(btRclMan);
            Controls.Add(btRunVIR);
            Margin = new Padding(4, 5, 4, 5);
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
