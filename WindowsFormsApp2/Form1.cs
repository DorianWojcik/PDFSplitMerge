using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Security;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter = "PDF (*.PDF)|*.PDF";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Load PDF";
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                richTextBox1.Text = "";
                // Read the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    richTextBox1.Text += file + "\n";
                }
            }
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            PdfDocument mergedDocument = new PdfDocument();

            foreach (String file in openFileDialog1.FileNames)
            {
                PdfDocument temp = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                foreach (PdfPage page in temp.Pages)
                    mergedDocument.AddPage(page);
            }
            richTextBox2.Text = "Merging file...";
            mergedDocument.Save(openFileDialog1.FileName.Remove(openFileDialog1.FileName.Length - 4) + "-merged.pdf");
        }

        private void SplitButton_Click(object sender, EventArgs e)
        {
            // Read the files
            foreach (String file in openFileDialog1.FileNames)
            {
                richTextBox2.Text = "Splitting file...";
                try
                {
                    PdfDocument fullDoc = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < fullDoc.PageCount; i += ((int)numericUpDown1.Value))
                    {
                        if (numericUpDown1.Value > 1)
                        {
                            int ind = ((int)numericUpDown1.Value);
                            ind -= 1;
                            PdfDocument newDoc = new PdfDocument();
                            for (int j = 0; j <= ind; j++)
                            {
                                if (j + i < fullDoc.PageCount)
                                {
                                    newDoc.AddPage(fullDoc.Pages[j + i]);
                                }
                            }
                            newDoc.Save(file.Remove(file.Length - 4) + "-" + i.ToString() + ".pdf");
                        }
                        else
                        {
                            PdfDocument newDoc = new PdfDocument();
                            newDoc.AddPage(fullDoc.Pages[i]);
                            newDoc.Save(file.Remove(file.Length - 4) + "-" + i.ToString() + ".pdf");
                        }
                    }
                }
                catch (SecurityException ex)
                {
                    // The user lacks appropriate permissions to read files, discover paths, etc.
                    MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                        "Error message: " + ex.Message + "\n\n" +
                        "Details (send to Support):\n\n" + ex.StackTrace
                    );
                }
                catch (Exception ex)
                {
                    // Could not load the image - probably related to Windows file system permissions.
                    MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                        + ". You may not have permission to read the file, or " +
                        "it may be corrupt.\n\nReported error: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
