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

namespace Talley_PA5
{
    public partial class Form1 : Form
    {
        // some useful variables
        private string name;
        private double color;
        private double ibu;
        private int count;
        private double sum;
        private string fileName;
        private string output;
        // helper methods
        private double IsDouble(string x, string field)
        {
            if ( x != "")
            {
                double value;
                if (double.TryParse(x, out value) == true)
                {
                    return value;
                }
                else
                {
                    MessageBox.Show(String.Format("{0} is not a valid {1}. The {1} field will be ignored for this search.", x, field));
                    return -100;
                }
            }
            else
            {
                return -100;
            }
        }
        private bool CheckFileName()
        {
            if (fileName == "")
            {
                MessageBox.Show("Please select file first");
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool ValidSearch()
        {
            if (nameBox.Text != "" || colorBox.Text != "" || ibuBox.Text != "")
            {
                return true;
            }
            else
            {
                MessageBox.Show("Please enter a search term");
                nameBox.Focus();
                return false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            // set variables to default or known bad values for testing.
            ibu = 0;
            color = 0;
            fileName = "";
            count = 0;
            sum = 0;
            output = "";
        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = "";
            // open dialog, check for valid file
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                // make sure file is correct file
                using (var sr = new StreamReader(fileName))
                {
                    string line = sr.ReadLine();
                    string[] isvalid = line.Split(',');
                    if (isvalid.Length == 22 && isvalid[4] == "StyleID" && isvalid[8] == "ABV" && isvalid[21] == "PrimingAmount")
                    {
                        // assume correct file (although this is potentially flawed)
                    }
                    else
                    {
                        // tell user to select correct file
                        MessageBox.Show("This is not the correct file. Please select the correct hombrewRecipieData file.");
                        return;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // check for file selection, make sure user has entered search term
            if (!CheckFileName() || !ValidSearch())
            {
                return;
            }
            // try and parse IBU and Color, set to -100 if unable to parse and inform user field is being ignored.
            ibu = IsDouble(ibuBox.Text, "IBU");
            color = IsDouble(colorBox.Text, "Color");
            // take in some user input, make the search case insensitive
            name = nameBox.Text.ToLower();
            name = name.Replace(" ", "");
            // start streamreader
            using (var sr = new StreamReader(fileName))
            {
                // reset count, sum, and output accumulators
                count = 0;
                sum = 0;
                output = "";
                // main loop to cycle through each entry
                while (!sr.EndOfStream)
                {
                    // read line
                    string line = sr.ReadLine();
                    // process line
                    line = line.Replace("N/A", "");
                    // split line
                    string[] data = line.Split(',');
                    // put into workable pieces, declare doubles to parse into
                    string dataName = data[1];
                    string dataURL = data[2];
                    string dataStyle = data[3];
                    string tempIBU = data[9];
                    string tempColor = data[10];
                    string tempABV = data[8];
                    double dataIBU;
                    double dataColor;
                    double dataABV = 0; 
                    // parse IBU, Color, and ABV to doubles
                    if (double.TryParse(tempIBU, out dataIBU) == false)
                    {
                        dataIBU = -10000;
                    }
                    if (double.TryParse(tempColor, out dataColor) == false)
                    {
                        dataColor = -10000;
                    }
                    if (tempABV != "")
                    {
                        double.TryParse(tempABV, out dataABV);
                    }
                    // now search for a match
                    // most restrictive first, must match IBU, Color, and Name or Style
                    if (ibu != -100 && color != -100 && name != "")
                    {
                        if (dataName.ToLower().Contains(name) && dataColor >= color - 2 && dataColor <= color + 2 && dataIBU >= ibu - 10 && dataIBU <= ibu + 10 || dataStyle.ToLower().Contains(name) && dataColor >= color - 2 && dataColor <= color + 2 && dataIBU >= ibu - 10 && dataIBU <= ibu + 10)
                        {
                            // count and sum it
                            count++;
                            sum += dataABV;
                            // store string output for later
                            output += String.Format("Name: {0}\tStyle: {1}\n URL: {2}\n Color: {3}\tIBU:{4}\tABV: {5}\n\n", dataName, dataStyle, dataURL, tempColor, tempIBU, tempABV);
                        }
                    }
                    // now match IBU and Name or Style
                    else if (ibu != -100 && name != "")
                    {
                        if (dataName.ToLower().Contains(name) && dataIBU >= ibu - 10 && dataIBU <= ibu + 10 || dataStyle.ToLower().Contains(name) && dataIBU >= ibu - 10 && dataIBU <= ibu + 10)
                        {
                            // count and sum it
                            count++;
                            sum += dataABV;
                            // store string output for later
                            output += String.Format("Name: {0}\tStyle: {1}\n URL: {2}\n Color: {3}\tIBU:{4}\tABV: {5}\n\n", dataName, dataStyle, dataURL, tempColor, tempIBU, tempABV);
                        }
                    }
                    // now match IBU and Color, NO name or style
                    else if (ibu != -100 && color != -100)
                    {
                        if (dataColor >= color - 2 && dataColor <= color + 2 && dataIBU >= ibu - 10 && dataIBU <= ibu + 10 || dataColor >= color - 2 && dataColor <= color + 2 && dataIBU >= ibu - 10 && dataIBU <= ibu + 10)
                        {
                            // count and sum it
                            count++;
                            sum += dataABV;
                            // store string output for later
                            output += String.Format("Name: {0}\tStyle: {1}\n URL: {2}\n Color: {3}\tIBU:{4}\tABV: {5}\n\n", dataName, dataStyle, dataURL, tempColor, tempIBU, tempABV);
                        }
                    }
                    // now match Color and Name or Style
                    else if (color != -100 && name != "")
                    {
                        if (dataName.ToLower().Contains(name) && dataColor >= color - 2 && dataColor <= color + 2 || dataStyle.ToLower().Contains(name) && dataColor >= color - 2 && dataColor <= color + 2)
                        {
                            // count and sum it
                            count++;
                            sum += dataABV;
                            // store string output for later
                            output += String.Format("Name: {0}\tStyle: {1}\n URL: {2}\n Color: {3}\tIBU:{4}\tABV: {5}\n\n", dataName, dataStyle, dataURL, tempColor, tempIBU, tempABV);
                        }
                    }
                    // match anything
                    else if (name != "" && dataName.ToLower().Contains(name) || name != "" && dataStyle.ToLower().Contains(name) || dataColor >= color -2 && dataColor <= color +2 || dataIBU >= ibu -10 && dataIBU <= ibu +10 )
                    {
                        // count and sum it
                        count++;
                        sum += dataABV;
                        // store string output for later
                        output += String.Format("Name: {0}\tStyle: {1}\n URL: {2}\n Color: {3}\tIBU:{4}\tABV: {5}\n\n", dataName, dataStyle, dataURL, tempColor, tempIBU, tempABV);
                    }

                }
            }
            // now do output conditions if less than 50 and more than 1 result
            if (count <= 50 && count >= 1)
            {
                resultBox.Text = String.Format("Found {0} results.\nAverage ABV is {1:F}\n\n", count, sum/count);
                // past in output
                resultBox.Text += output;
                // clear sum, output, and count values, clear search boxes, reset focus to name
                sum = 0;
                count = 0;
                output = "";
                nameBox.Clear();
                ibuBox.Clear();
                colorBox.Clear();
                nameBox.Focus();
            }
            // output when no result is returned
            else if (count == 0)
            {
                resultBox.Text = "Found no results.";
                // clear sum, output, and count values, clear search boxes, reset focus to name
                sum = 0;
                count = 0;
                output = "";
                nameBox.Clear();
                ibuBox.Clear();
                colorBox.Clear();
                nameBox.Focus();
            }
            // output when more than 50 results are returned.
            else
            {
                resultBox.Text = String.Format("Found {0} results. Average ABV is {1:F}\n\nToo many results. Please refine your search.", count, sum / count);
                // clear sum, output, and count values, clear search boxes, reset focus to name
                sum = 0;
                count = 0;
                output = "";
                nameBox.Clear();
                ibuBox.Clear();
                colorBox.Clear();
                nameBox.Focus();
            }
        }
    }
}
