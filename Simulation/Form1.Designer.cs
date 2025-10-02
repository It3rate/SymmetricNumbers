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
			btRun = new Button();
			tbOutput = new TextBox();
			SuspendLayout();
			// 
			// btRun
			// 
			btRun.Location = new Point(40, 40);
			btRun.Name = "btRun";
			btRun.Size = new Size(75, 23);
			btRun.TabIndex = 0;
			btRun.Text = "Run";
			btRun.UseVisualStyleBackColor = true;
			btRun.Click += btRun_Click;
			// 
			// tbOutput
			// 
			tbOutput.Location = new Point(246, 28);
			tbOutput.Multiline = true;
			tbOutput.Name = "tbOutput";
			tbOutput.Size = new Size(450, 373);
			tbOutput.TabIndex = 1;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(tbOutput);
			Controls.Add(btRun);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button btRun;
		private TextBox tbOutput;
	}
}
