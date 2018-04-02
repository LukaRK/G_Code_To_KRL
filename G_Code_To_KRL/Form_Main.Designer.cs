namespace G_Code_To_KRL
{
    partial class Form_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bt_SelectInputFile = new System.Windows.Forms.Button();
            this.rtb_InputFile = new System.Windows.Forms.RichTextBox();
            this.rtb_OutputFile = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // bt_SelectInputFile
            // 
            this.bt_SelectInputFile.Location = new System.Drawing.Point(236, 570);
            this.bt_SelectInputFile.Name = "bt_SelectInputFile";
            this.bt_SelectInputFile.Size = new System.Drawing.Size(95, 49);
            this.bt_SelectInputFile.TabIndex = 1;
            this.bt_SelectInputFile.Text = "button1";
            this.bt_SelectInputFile.UseVisualStyleBackColor = true;
            this.bt_SelectInputFile.Click += new System.EventHandler(this.bt_SelectInputFile_Click);
            // 
            // rtb_InputFile
            // 
            this.rtb_InputFile.Location = new System.Drawing.Point(33, 25);
            this.rtb_InputFile.Name = "rtb_InputFile";
            this.rtb_InputFile.Size = new System.Drawing.Size(298, 283);
            this.rtb_InputFile.TabIndex = 2;
            this.rtb_InputFile.Text = "";
            // 
            // rtb_OutputFile
            // 
            this.rtb_OutputFile.Location = new System.Drawing.Point(721, 25);
            this.rtb_OutputFile.Name = "rtb_OutputFile";
            this.rtb_OutputFile.Size = new System.Drawing.Size(298, 283);
            this.rtb_OutputFile.TabIndex = 3;
            this.rtb_OutputFile.Text = "";
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 642);
            this.Controls.Add(this.rtb_OutputFile);
            this.Controls.Add(this.rtb_InputFile);
            this.Controls.Add(this.bt_SelectInputFile);
            this.Name = "Form_Main";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bt_SelectInputFile;
        private System.Windows.Forms.RichTextBox rtb_InputFile;
        private System.Windows.Forms.RichTextBox rtb_OutputFile;
    }
}

