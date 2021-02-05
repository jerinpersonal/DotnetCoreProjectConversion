
namespace DotnetMigratorUI
{
    partial class frmDotnetMigrator
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtProjectFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnMigration = new System.Windows.Forms.Button();
            this.btnWebConfigMigration = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtProjectFilePath
            // 
            this.txtProjectFilePath.Location = new System.Drawing.Point(94, 57);
            this.txtProjectFilePath.Name = "txtProjectFilePath";
            this.txtProjectFilePath.Size = new System.Drawing.Size(198, 20);
            this.txtProjectFilePath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ProjectFile";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(310, 57);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnMigration
            // 
            this.btnMigration.Location = new System.Drawing.Point(262, 99);
            this.btnMigration.Name = "btnMigration";
            this.btnMigration.Size = new System.Drawing.Size(123, 23);
            this.btnMigration.TabIndex = 3;
            this.btnMigration.Text = "Execute Migration";
            this.btnMigration.UseVisualStyleBackColor = true;
            this.btnMigration.Click += new System.EventHandler(this.btnMigration_Click);
            // 
            // btnWebConfigMigration
            // 
            this.btnWebConfigMigration.Location = new System.Drawing.Point(262, 139);
            this.btnWebConfigMigration.Name = "btnWebConfigMigration";
            this.btnWebConfigMigration.Size = new System.Drawing.Size(123, 23);
            this.btnWebConfigMigration.TabIndex = 4;
            this.btnWebConfigMigration.Text = "Web Config Migration";
            this.btnWebConfigMigration.UseVisualStyleBackColor = true;
            this.btnWebConfigMigration.Click += new System.EventHandler(this.btnWebConfigMigration_Click);
            // 
            // frmDotnetMigrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 174);
            this.Controls.Add(this.btnWebConfigMigration);
            this.Controls.Add(this.btnMigration);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtProjectFilePath);
            this.Name = "frmDotnetMigrator";
            this.Text = "Dotnet Core Migrator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtProjectFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnMigration;
        private System.Windows.Forms.Button btnWebConfigMigration;
    }
}

