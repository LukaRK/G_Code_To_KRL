using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private string FormSpecLinXY = "LIN {{FRAME: X {0}, Y {1}}}\r\n";
        private string FormSpecLinZ = "LIN {FRAME: Z %4.3f}\r\n";
        private string FormSpecVel = "BAS( #VEL_CP, %4.3f)\r\n";
        private string FormSpecCirc = "CIRC {X %4.3f, Y %4.3f,Z %4.3f},{X %4.3f, Y %4.3f,Z %4.3f} \r\n";
        private string FormSpecStart = "JET = TRUE \r\nABBR_EN = TRUE\r\n";
        private string FormSpecStop = "ABBR_EN = FALSE\r\nJET = FALSE \r\n";
        private string FormSpecShape = ";***Shape %u*** \r\n \r\n";
        private double[] LastExeCoord = new double[3];

        //--- Inputs ---
        private string FileName = "stegosauruskop8mm.ngc"; //FileName may not contain spaces
        private int ToolNo = 3;
        private int RefBase = 3;
        private double[] PurgePos = { 460, 600, -20 }; //XYZ-position where the waterjet waits for intensifier boot and purges air form its system
        private double[] MaxCoord = { 1935, 930, 20 }; //Maximum position of the positioning-system
        private double CircleRes = 0.1; //Minimum distance between 3 circle points in mm(repeatability = 0,06 mm)
        private int Circlecomp = 0; // Flag for active full circles

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

            // --- Standard text ---

            //Pre declaration block
            string FormSpecPreDec = "DEF {0}( ) \r\n\r\n";
            outputData = string.Concat(outputData, string.Format(FormSpecPreDec,FileName));

            //Declaration block
            string FormSpecDec = ";----- Declaration section ----- \r\nDECL E6AXIS home ;12 axis home point of THIS program do NOT confuse with Home or $H_Pos \r\nDECL FRAME My_Base     ;custom base \r\nSIGNAL JET $OUT[81] \r\nSIGNAL ABBR_EN $OUT[82] \r\n\r\n";
            outputData = string.Concat(outputData, FormSpecDec);

            //Initialisation block
            string FormSpecIni = ";----- Initialization ----- \r\n;FOLD INI \r\n  ;FOLD BASISTECH INI \r\n    BAS (#INITMOV,0 ) \r\n  ;ENDFOLD (BASISTECH INI) \r\n  ;FOLD USER INI \r\n   ;Make your modifications here \r\n \r\n  ;ENDFOLD (USER INI) \r\n;ENDFOLD (INI) \r\nBAS(#TOOL,{0})        ;Selecting tool \r\nBAS(#BASE,{1})        ;Selecting base \r\nhome = {{E6AXIS: A1 45,A2 -90,A3 90,A4 -90,A5 -45,A6 30,E1 0,E2 0,E3 0,E4 0,E5 0,E6 0}} \r\n\r\n";
            outputData = string.Concat(outputData, string.Format(FormSpecIni,ToolNo,RefBase));

            //Main block
            string FormSpecMain = ";----- Main Section ----- \r\nBAS (#VEL_CP, 0.3) \r\nBAS (#VEL_PTP, 100) \r\nJET = FALSE \r\nABBR_EN = FALSE \r\nPTP home ;Point-to-point movement at default(100%) speed and velocity \r\n\r\nLIN {{FRAME: X {0}, Y {1}, Z {2}}}\r\nJET = TRUE \r\nHALT ;MANUALLY TURN ON INTENSIFIER \r\nJET = FALSE \r\n\r\n";
            outputData = string.Concat(outputData, string.Format(FormSpecMain,PurgePos[0],PurgePos[1],PurgePos[2]));

            // --- Condition checks --- NO ANGLE DISPLAYED

            rtb_OutputFile.Text = outputData;

            foreach (string i in data)
            {
                if (i.Length > 3) //Avoid out of range exception in string.substring
                {
                    if (i.Substring(0, 4).Contains("G0 X"))
                    {
                        Match m = Regex.Match(i, "X(?<X>[-]?\\d*\\.?\\d*).Y(?<Y>[-]?\\d*\\.?\\d*).Z(?<Z>[-]?\\d*\\.?\\d*)"); //Match X0.00 Y0.00 Z0.00 and place numbers and sign (here 0.00) in between in group X,Y,Z respectively
                        if (m.Success)
                        {
                            outputData = string.Concat(outputData, string.Format(FormSpecLinXY, m.Groups["X"].Value, m.Groups["Y"].Value));
                            //Console.WriteLine("test X " + m.Groups["X"] + " Y " + m.Groups["Y"] + " Z " + m.Groups["Z"]);
                        }
                    }
                }
            }

            rtb_OutputFile.Text = outputData;
        }
    }
}
