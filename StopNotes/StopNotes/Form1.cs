/*Author: MikeJRthe2nd
Date: 1/9/2020
Program: A stopwatch program that saves the final time the stopwatch ends at to a text file. 
        Also have it so that when a key is pressed it saves the time when that key was pressed and puts that on the text file too.

        Note: Was called StopNotes, but another program has that name so I switched it to StopwatchNotes. Doesn't roll
        off the tounge as well, but it is easier to understand. 

        Things to add:

        -The extra timer only goes to anything below 24 hours, but can handle higher ammounts if needed.


        -Set timestamp to a key of the users choice. Since this needs to be used as a globel key it currently only
            works with the ? key. Might add ability to change hotkey later. 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Stopwatch
using System.Diagnostics;
using System.Threading;

using System.Runtime.InteropServices; //Windows API
//File
using System.IO;

namespace StopNotes
{
    public partial class Form1 : Form
    {
        Stopwatch stopNotes = new Stopwatch();
        String noteTime = "";
        long extra; //For adding extra time
        bool resetClick = false;

        private IntPtr thisForm;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(String theClassName, String theAppName);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id );

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thisForm = FindWindow(null, "StopwatchNotes");

            RegisterHotKey(thisForm, 1, (uint) 0, (uint) Keys.OemQuestion);

        }


        //Starts the timer
        private void button1_Click(object sender, EventArgs e)
        {
            
            stopNotes.Start(); //Starts timer
            timer1.Start();

            //Turns off these buttons when the timer is going
            buttonReset.Enabled = false;
            buttonSave.Enabled = false;
            textBox1.Enabled = false;
            numericUpDownMinutes.Enabled = false;
            numericUpDownSeconds.Enabled = false;

            //Sets extra to be the converted number of ticks per second and min.
            extra = ((long)numericUpDownMinutes.Value * 600000000) +    //1 minute is 600 million ticks
                    ((long)numericUpDownSeconds.Value * 10000000);        //1 second is 10 million ticks

        }

        //Stops the Timer
        private void button2_Click(object sender, EventArgs e)
        {
            //Stops the timers
            stopNotes.Stop(); 
            timer1.Stop();

            //Turns on these buttons when the timer has stopped 
            buttonReset.Enabled = true;
            buttonSave.Enabled = true;
            textBox1.Enabled = true;
            numericUpDownMinutes.Enabled = true;
            numericUpDownSeconds.Enabled = true;


            //I think this needs to be added to the textbox Does nothing as of now.
            TimeSpan ts = (stopNotes.Elapsed + new TimeSpan(extra));
            String totalTime = ts.Hours.ToString().PadLeft(2,'0') + ":" + 
                ts.Minutes.ToString().PadLeft(2, '0') + ":" + 
                ts.Seconds.ToString().PadLeft(2, '0'); //Formats time

        }

        //Timestamps the current time into a string or text file
        private void button3_Click(object sender, EventArgs e)
        {
            
            TimeSpan ts2 = (stopNotes.Elapsed + new TimeSpan(extra));
            noteTime = noteTime + "\n~" + ts2.Hours.ToString().PadLeft(2, '0') + ":" +
                ts2.Minutes.ToString().PadLeft(2, '0') + ":" +
                ts2.Seconds.ToString().PadLeft(2, '0'); //Formats time

            textBox1.Text = noteTime; //saves time to textbox

            //Scrolls to bottom of textbox
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
            
        }

        //Save to text file. Copied most of this.
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Stream save = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                using (StreamWriter saveW = new StreamWriter(save))
                {
                    saveW.Write(textBox1.Text);
                }
            }
        }

        //Textbox will save what's put into it.
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //If reset was clicked then don't add the spaces
            if (resetClick == true)
            {
                resetClick = false;
            }
            else
            {
                noteTime = textBox1.Text + "\r\n";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            TimeSpan ts3 = (stopNotes.Elapsed + new TimeSpan(extra));

            //Timer displays
            mainTimerLabel.Text = ts3.Hours.ToString().PadLeft(2, '0') + ":" +
                ts3.Minutes.ToString().PadLeft(2, '0') + ":" +
                ts3.Seconds.ToString().PadLeft(2, '0'); //Formats time
        }

        //Resets the timer and clears the timestamps.
        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure? This will delete your timestamps.", "Confirm Reset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                stopNotes.Reset();
                timer1.Dispose();
                noteTime = "";
                mainTimerLabel.Text = "00:00:00";
                resetClick = true;
                textBox1.Text = noteTime; //saves time to textbox
            }
        }

        //Unregisters the Hotkey when the window is closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(thisForm, 1);
        }

        //Tells the program what to do when a hotkey is pressed
        protected override void WndProc(ref Message keyPressed)
        {
            if(keyPressed.Msg == 0x0312 )
            {
                buttonNote.PerformClick(); //Click the timestamp button
            }
            base.WndProc(ref keyPressed);
        }


        //Hotkey Asignment
        public enum fsModifiers
        {
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Window = 0x0008,
        }
    }
}
