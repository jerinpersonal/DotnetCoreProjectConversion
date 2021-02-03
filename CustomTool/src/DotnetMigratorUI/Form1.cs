using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotnetMigratorUI
{
    public partial class frmDotnetMigrator : Form
    {
        public frmDotnetMigrator()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Csharp Project File",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csproj",
                Filter = "Csharp Projects (*.csproj)|*.csproj",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtProjectFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnMigration_Click(object sender, EventArgs e)
        {
            try
            {
                Migrator.Execute(txtProjectFilePath.Text, @"DotnetCore31Template.csproj");
                MessageBox.Show("Migration Has Been Successfully Completed.");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Migration Has Been Failed");
            }
        }
    }
}
