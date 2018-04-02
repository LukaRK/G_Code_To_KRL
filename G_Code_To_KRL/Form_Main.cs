using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace G_Code_To_KRL
{
    public partial class Form_Main : Form
    {
        private FolderBrowserDialog folderBrowserDialog1;
        private OpenFileDialog openFileDialog1;

        private bool fileOpened = false;

        private string openFileName;
        private string inputData;
        private string[] data;
        public string outputData;


        public Form_Main()
        {
            InitializeComponent();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            this.openFileDialog1.DefaultExt = "txt";
            this.openFileDialog1.Filter = "txt files (*.txt)|*.txt";

            // Set the help text description for the FolderBrowserDialog.
            this.folderBrowserDialog1.Description =
                "Select the directory that you want to use as the default.";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            this.folderBrowserDialog1.ShowNewFolderButton = false;

            // Default to the My Documents folder.
            this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
        }

        private void bt_SelectInputFile_Click(object sender, EventArgs e)
        {
            // If a file is not opened, then set the initial directory to the
            // FolderBrowserDialog.SelectedPath value.
            if (!fileOpened)
            {
                openFileDialog1.InitialDirectory = folderBrowserDialog1.SelectedPath;
                openFileDialog1.FileName = null;
            }

            // Display the openFile dialog.
            DialogResult result = openFileDialog1.ShowDialog();

            // OK button was pressed.
            if (result == DialogResult.OK)
            {
                openFileName = openFileDialog1.FileName;
                try
                {
                    // Output the requested file in richTextBox1.
                    Stream s = openFileDialog1.OpenFile();
                    byte[] bytes = new byte[s.Length];
                    s.Position = 0;
                    s.Read(bytes, 0, (int)s.Length);
                    inputData = Encoding.ASCII.GetString(bytes);
                    rtb_InputFile.Text = inputData;
                    data = rtb_InputFile.Text.Split(new[] { "\n", "\r", "\r\n"},StringSplitOptions.None);
                    s.Close();

                    fileOpened = true;

                }
                catch (Exception exp)
                {
                    MessageBox.Show("An error occurred while attempting to load the file. The error is:"
                                    + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine);
                    fileOpened = false;
                }
                Invalidate();

                //closeMenuItem.Enabled = fileOpened;
            }

            // Cancel button was pressed.
            else if (result == DialogResult.Cancel)
            {
                return;
            }

            // --- Standaard text ---

            //Pre declaration block
            string FormSpecPreDec = "&ACCESS RVP \r\n&REL 1 \r\n&PARAM TEMPLATE = C:\\KRC\\Roboter\\Template\\vorgabe \r\n&PARAM EDITMASK = * \r\nDEF %s( ) \r\n\r\n"; // '\\' = '\' without declaring a command" 
            outputData = string.Concat(outputData, FormSpecPreDec);

            //Declaration block
            string FormSpecDec = ";----- Declaration section ----- \r\nDECL E6AXIS home ;12 axis home point of THIS program do NOT confuse with Home or $H_Pos \r\nDECL FRAME My_Base     ;custom base \r\nSIGNAL JET $OUT[81] \r\nSIGNAL ABBR_EN $OUT[82] \r\n\r\n";
            outputData = string.Concat(outputData, FormSpecDec);

            //Initialisation block
            string FormSpecIni = ";----- Initialization ----- \r\n;FOLD INI \r\n  ;FOLD BASISTECH INI \r\n    GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )\r\n    INTERRUPT ON 3 \r\n    BAS (#INITMOV,0 ) \r\n  ;ENDFOLD (BASISTECH INI) \r\n  ;FOLD USER INI \r\n   ;Make your modifications here \r\n \r\n  ;ENDFOLD (USER INI) \r\n;ENDFOLD (INI) \r\nBAS(#TOOL,%i)        ;Selecting tool \r\nBAS(#BASE,%i)        ;Selecting base \r\nhome = {E6AXIS: A1 45,A2 -90,A3 90,A4 -90,A5 -45,A6 30,E1 0,E2 0,E3 0,E4 0,E5 0,E6 0} \r\n\r\n";
            outputData = string.Concat(outputData, FormSpecIni);

            //Main block
            string FormSpecMain = ";----- Main Section ----- \r\nBAS (#VEL_CP, 0.3) \r\nBAS (#VEL_PTP, 100) \r\nJET = FALSE \r\nABBR_EN = FALSE \r\nPTP home ;Point-to-point movement at default(100%%) speed and velocity \r\n\r\nLIN {FRAME: X %4.3f, Y %4.3f, Z %4.3f}\r\nJET = TRUE \r\nHALT ;MANUALLY TURN ON INTENSIFIER \r\nJET = FALSE \r\n\r\n";
            outputData = string.Concat(outputData, FormSpecMain);

            // --- Condition checks --- NO ANGLE DISPLAYED

            rtb_OutputFile.Text = outputData;

            foreach (string i in data)
                {
                    
                }
        }
    }
}
