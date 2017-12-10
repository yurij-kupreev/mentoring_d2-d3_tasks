namespace WindowsAppMemoryLeak
{
    partial class frmMemoryLeakage
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
            this.components = new System.ComponentModel.Container();
            this.btnUnManagedLeak = new System.Windows.Forms.Button();
            this.btnManagedLeak = new System.Windows.Forms.Button();
            this.timerUnManaged = new System.Windows.Forms.Timer(this.components);
            this.timerManaged = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnUnManagedLeak
            // 
            this.btnUnManagedLeak.Location = new System.Drawing.Point(12, 12);
            this.btnUnManagedLeak.Name = "btnUnManagedLeak";
            this.btnUnManagedLeak.Size = new System.Drawing.Size(161, 35);
            this.btnUnManagedLeak.TabIndex = 0;
            this.btnUnManagedLeak.Text = "Start UnManaged Leak";
            this.btnUnManagedLeak.UseVisualStyleBackColor = true;
            this.btnUnManagedLeak.Click += new System.EventHandler(this.btnUnManagedLeak_Click);
            // 
            // btnManagedLeak
            // 
            this.btnManagedLeak.Location = new System.Drawing.Point(195, 12);
            this.btnManagedLeak.Name = "btnManagedLeak";
            this.btnManagedLeak.Size = new System.Drawing.Size(161, 35);
            this.btnManagedLeak.TabIndex = 1;
            this.btnManagedLeak.Text = "Start Managed Leak";
            this.btnManagedLeak.UseVisualStyleBackColor = true;
            this.btnManagedLeak.Click += new System.EventHandler(this.btnManagedLeak_Click);
            // 
            // timerUnManaged
            // 
            this.timerUnManaged.Tick += new System.EventHandler(this.timerUnManaged_Tick);
            // 
            // timerManaged
            // 
            this.timerManaged.Tick += new System.EventHandler(this.timerManaged_Tick);
            // 
            // frmMemoryLeakage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 62);
            this.Controls.Add(this.btnManagedLeak);
            this.Controls.Add(this.btnUnManagedLeak);
            this.Name = "frmMemoryLeakage";
            this.Text = "Memory Leakage Testing";
            this.Load += new System.EventHandler(this.frmMemoryLeakage_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUnManagedLeak;
        private System.Windows.Forms.Button btnManagedLeak;
        private System.Windows.Forms.Timer timerUnManaged;
        private System.Windows.Forms.Timer timerManaged;
    }
}

