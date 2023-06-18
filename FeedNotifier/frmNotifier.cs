using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FeedNotifier
{
    public partial class frmNotifier : Form
    {
        DateTime dtLastUpdate = DateTime.Now;
        bool isFirstTimeLoad = true;
        bool offTimer=false;
        public frmNotifier()
        {
            InitializeComponent();
        }

        private void btnGetFeed_Click(object sender, EventArgs e)
        {
            GetData();
        }        

        private void frmNotifier_Load(object sender, EventArgs e)
        {
            notify("Feed Notifier", "Feed", "Notification appears here");          

            timer1.Start();
            this.Close();    

        }      

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

            this.Show();
            notifyIcon1.Visible = false;
            this.BringToFront();
        }

        private void frmNotifier_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            Application.Exit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {                
                if (e.ColumnIndex == 3)
                {
                    string txt = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                    if (IsValidURL(txt))
                    {
                        Process.Start(txt);
                        return;
                    }
                }                
           }
            catch (Exception ex)
            {
                clearAll();
                toLog("Error: " + ex.Message);
            }            

        }
        bool IsValidURL(string URL)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URL);
        }
        private void clearAll()
        {
            isFirstTimeLoad = true;
            dtLastUpdate = DateTime.Now;
            dataGridView1.Rows.Clear();
            btnGetFeed.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!offTimer)
            {             
                GetData();                
            }
        }
        private void GetData()
        {
            offTimer = true;
            DateTime dtNow = DateTime.Now;
            try
            {
                btnGetFeed.Enabled = false;
                string url = txtFeedUrl.Text.Trim(); 
                if (url.Trim() == "")
                {
                    clearAll();
                    toLog("Error: Please enter an url for RSS Feed");
                    return;
                }

                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                foreach (SyndicationItem item in feed.Items)
                {
                    string title = item.Title.Text;
                    string summary = item.Summary.Text;
                    string publishDate = item.PublishDate.DateTime.ToString("dd-MMM-yyyy hh:mm tt"); 
                    //SyndicationContent syC = item.Content;
                    string uriValue = item.Links[0].Uri.AbsoluteUri;

                    if (isFirstTimeLoad)
                    {
                        //Load all the the post in 24 hours
                        TimeSpan ts = dtNow.Subtract(item.PublishDate.DateTime);
                        double reminderMinute = ts.TotalMinutes;
                        if (reminderMinute < 1440)
                        {
                            string[] row0 = { title, summary, publishDate, uriValue };
                            dataGridView1.Rows.Add(row0);
                        }

                    }
                    else if (item.PublishDate.DateTime > dtLastUpdate)// get all the new posts after the last update
                    {
                        notify(title, summary, publishDate);
                        speakOut(title);
                        string[] row0 = { title, summary, publishDate, uriValue };
                        dataGridView1.Rows.Add(row0);
                    }

                }
                isFirstTimeLoad = false;
            }
            catch (Exception ex)
            {
                clearAll();
                toLog("Error: " + ex.Message);
            }
            finally
            {
                dtLastUpdate = dtNow;
                btnGetFeed.Enabled = true;
                offTimer = false;
            }

        }
        void toLog(string output)
        {
            richTextBox1.AppendText("\r\n" + output);
            richTextBox1.ScrollToCaret();
        }

        //Speak the title asynchronusly
        private void speakOut(string text)
        {
            try
            {
                System.Speech.Synthesis.SpeechSynthesizer speechSynthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
                string selectedVoice = "Microsoft David Desktop";
                speechSynthesizer.SelectVoice(selectedVoice);
                speechSynthesizer.SpeakAsync(text);
                //speechSynthesizer.Rate = -2;
            }
            catch (Exception ex)
            {               
                toLog("Error on speech: " + ex.Message);
            }
        }
        private void notify(string text, string tipTitle, string tipText)
        {
            try
            {                
                notifyIcon1.Text = text;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500, tipTitle, tipText, ToolTipIcon.Info);
                notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);
                this.Hide();
            }
            catch (Exception ex)
            {
                clearAll();
                toLog("Error: " + ex.Message);
            }            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearAll();
        }       

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.StateChanged != DataGridViewElementStates.Selected) return;

            string title = "<b>Title: " + dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() + "</b> </br>";
            string txt = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string dtime = "</br></br><b>Published on: </b>" + dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            string lnk = "</br><b>Link: </b>" + dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

            frmSummary formSummary = new frmSummary("<html>" + title + txt + dtime + lnk + "</html>");
            formSummary.ShowDialog();
        }
    }
}
