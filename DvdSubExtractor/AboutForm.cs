using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace DvdSubExtractor;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();

        this.aboutTextBox.Text = "DVD Subtitle Extractor " + Application.ProductVersion + "\n" +
            "Copyright © 2009-2012 Christopher R Meadowcroft\n" +
            "Copyright © 2026 Daisy\n\n" +
            "Licensed under the GNU General Public License v2.0";
    }

    private void licenseButton_Click(object sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://choosealicense.com/licenses/gpl-2.0/",
            UseShellExecute = true
        });
    }
}

