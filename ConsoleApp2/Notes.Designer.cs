namespace ConsoleApp2
{
    partial class Notes
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
            this.txtnotes = new System.Windows.Forms.TextBox();
            this.btnstamp = new System.Windows.Forms.Button();
            this.btncancel = new System.Windows.Forms.Button();
            this.btnok = new System.Windows.Forms.Button();
            this.btnclear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtnotes
            // 
            this.txtnotes.Location = new System.Drawing.Point(12, 12);
            this.txtnotes.Multiline = true;
            this.txtnotes.Name = "txtnotes";
            this.txtnotes.Size = new System.Drawing.Size(676, 226);
            this.txtnotes.TabIndex = 0;
            // 
            // btnstamp
            // 
            this.btnstamp.Location = new System.Drawing.Point(12, 254);
            this.btnstamp.Name = "btnstamp";
            this.btnstamp.Size = new System.Drawing.Size(110, 31);
            this.btnstamp.TabIndex = 1;
            this.btnstamp.Text = "User Stamp";
            this.btnstamp.UseVisualStyleBackColor = true;
            this.btnstamp.Click += new System.EventHandler(this.btnstamp_Click);
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(578, 309);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(110, 31);
            this.btncancel.TabIndex = 2;
            this.btncancel.Text = "Cancel";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // btnok
            // 
            this.btnok.Location = new System.Drawing.Point(462, 309);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(110, 31);
            this.btnok.TabIndex = 3;
            this.btnok.Text = "Ok";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnclear
            // 
            this.btnclear.Location = new System.Drawing.Point(128, 254);
            this.btnclear.Name = "btnclear";
            this.btnclear.Size = new System.Drawing.Size(110, 31);
            this.btnclear.TabIndex = 4;
            this.btnclear.Text = "Clear";
            this.btnclear.UseVisualStyleBackColor = true;
            this.btnclear.Click += new System.EventHandler(this.btnclear_Click);
            // 
            // Notes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 352);
            this.Controls.Add(this.btnclear);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnstamp);
            this.Controls.Add(this.txtnotes);
            this.Name = "Notes";
            this.Text = "Classification Notes";
            this.Load += new System.EventHandler(this.Notes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtnotes;
        private System.Windows.Forms.Button btnstamp;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.Button btnclear;
    }
}