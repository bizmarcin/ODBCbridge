using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using static System.Windows.Forms.DataFormats;

namespace bridgeODBC
{



    public partial class connectButton : Form
    {

        public connectButton()
        {
            InitializeComponent();
        }

     

        private string TimeConvert(string TimeRaw)
        {
            string mounth = null;
            string day = null;
            string year = null;
            string hour = null;
            string minute = null;
            string second = null;
            string datatime = null;

            int i = 0;

            foreach (char cha in TimeRaw)
            {                
                if (i == 5)
                {
                        second = second + cha;
                }


                if (i == 4)
                {
                    if (cha == ':')
                    {
                        i = 5;
                    }
                    else
                    {
                        minute = minute + cha;
                    }
                }

                if (i == 3)
                {
                    if (cha == ':')
                    {
                        i = 4;
                    }
                    else
                    {
                        hour = hour + cha;
                    }
                }

                if (i == 2)
                {
                    if (cha == ' ')
                    {
                        i = 3;
                    }
                    else
                    {
                        year = year + cha;
                    }
                }

                if (i == 1)
                {
                    if (cha == '/')
                    {
                        i = 2;
                    }
                    else
                    {
                        day = day + cha;
                    }
                }

                if (i == 0)
                {
                    if (cha == '/')
                    {
                        i = 1;
                    }
                    else
                    {
                        mounth = mounth + cha;
                    }
                }

            }

            datatime = year + "-" + mounth + "-" + day + " " + hour + ":" + minute + ":" + second;
            return datatime;
        }

        private void ReadDataTable(string connectionString, string queryString)
        {
            //czytanie tabeli #######################################################
            DataSet ds = new DataSet();

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection);
                    OdbcCommandBuilder cmdb = new OdbcCommandBuilder(adapter);

                    adapter.Fill(ds, "Event2");
                    dataGridView1.DataSource = ds.Tables["Event2"];

                    adapter.Dispose();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + ex.Message);
                    LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " Blad wystapil przy minimalnym EventID= " + eventIdTextBox.Text);
                }


                

            }
        }



        private bool ReWrite()
        {
            int writeCounter = 0;
            int errorCounter = 0;
            reWriteTimer.Stop();
            reWriteTimer.Enabled = false;

            //ReadDataTable("DSN=" + comboDSN.Text + "; Integrated Security=SSPI", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>"+eventIdTextBox.Text);

            switch (rewComboBox.Text)
            {
                case "R410":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Integrated Security=SSPI", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
                    break;
                case "R430":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Integrated Security=SSPI", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
                    break;
                case "R431":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Uid=" + uidTextBox.Text + "; Pwd=" + pwdTextBox.Text + ";", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
                    break;
                default:
                    ReadDataTable("DSN=" + comboDSN.Text + "; Uid=" + uidTextBox.Text + "; Pwd=" + pwdTextBox.Text + ";", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
                    //ReadDataTable("DSN=EXPDataSource; Uid=lotos; Pwd=lotos;", "SELECT Action, Actor, AlarmLimit, AreaCode, AreaName, Block, Category, ChangedTime, Comment, ConditionName, Description, EventID, GDAServerName, LocationTagName, PrevValue, Priority, Source, SourceEntityName, Station, Time, TransactionID, Units, Value FROM \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
                    break;
            }
            LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " \"Event2\" WHERE EventID>" + eventIdTextBox.Text);
            dataGridView1.Columns[7].ValueType = typeof(DateTime);
            dataGridView1.Columns[19].ValueType = typeof(DateTime);
            string rowdata = null;
            float maxEvent = float.Parse(eventNoNumericUpDown.Value.ToString()) + float.Parse(eventIdTextBox.Text);

            //open transaction mysql
            MySql.Data.MySqlClient.MySqlConnection connectionSQL;
            string myConnectionString;
            
            myConnectionString="Server=" + serverName.Text + "; UID=" + userTextBox.Text + "; Pwd=" + pwdTextBox.Text + "; database=" + dbNameTextBox.Text + ";";


            connectionSQL = new MySql.Data.MySqlClient.MySqlConnection();
            connectionSQL.ConnectionString = myConnectionString;
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();            
            //bylu tu

//////////////////////////////////////////////////////////



            try
                          {// przeniesione 4 linie
                            connectionSQL.Open();
                            LogListBox.Items.Insert(0, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Poczatek polaczenia MySQL");
                            reWriteTimer.Enabled = true;
                            reWriteTimer.Start();

                              cmd.Connection = connectionSQL;
                              LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- MySQL- conection OK");
                              cmd.CommandText = "START TRANSACTION;";
                              LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- MySQL- transaction OK");   
                              cmd.Prepare();
                              LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- MySQL- prepare OK");

                              cmd.ExecuteNonQuery();
                              LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- MySQL- query OK");
                          }
                          catch (Exception ex)
                          {
                              if (connectionSQL.State == ConnectionState.Open)
                              {
                                  connectionSQL.Close();
                                  LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Koniec polaczenia MySQL- error");
                                  reWriteTimer.Stop();
                                  reWriteTimer.Enabled = false;
                              }
                              LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + ex.Message);
                          }


            //poczatek petli do zapisywania
            for (int i = 0; i < (dataGridView1.Rows.Count - 1); i++)
            {
                if (writeCounter >= eventNoNumericUpDown.Value)
                {
                    break;
                }

                for (int j = 0; j < (dataGridView1.Columns.Count); j++)
                {
                    if (j > 0)
                    {
                        rowdata = rowdata + ",";
                    }


                    if(dataGridView1.Rows[i].Cells[j].Value.ToString() != string.Empty)
                    {
                        if (j == 7)
                        {
                            try
                            {
                                if ((rewComboBox.SelectedItem.ToString() == "R430") || (rewComboBox.SelectedItem.ToString() == "R431"))
                                {
                                    rowdata = rowdata + " '" + TimeConvert(dataGridView1.Rows[i].Cells[j].Value.ToString()) + "'";
                                }
                                else
                                {
                                    rowdata = rowdata + " '" + dataGridView1.Rows[i].Cells[j].Value.ToString() + "'";
                                }
                            }
                            catch (Exception Ee)
                            {
                                LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + Ee.ToString());
                                return false;
                            }
                        }
                        else
                        {
                            if (j == 19)
                            {
                                try
                                {
                                    if ((rewComboBox.SelectedItem.ToString() == "R430") || (rewComboBox.SelectedItem.ToString() == "R431"))
                                    {
                                        rowdata = rowdata + " '" + TimeConvert(dataGridView1.Rows[i].Cells[j].Value.ToString()) + "'";
                                    }
                                    else
                                    {
                                        rowdata = rowdata + " '" + dataGridView1.Rows[i].Cells[j].Value.ToString() + "'";
                                    }
                                        
                                }
                                catch (Exception Ee)
                                {
                                    LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + Ee.ToString());
                                    return false;
                                }
                            }
                            else
                            {
                                if (j == 11)
                                {
                                    try
                                    {
                                       if (float.Parse(dataGridView1.Rows[i].Cells[j].Value.ToString()) > float.Parse(eventIdTextBox.Text))
                                        {
                                            eventIdTextBox.Text = dataGridView1.Rows[i].Cells[j].Value.ToString();

                                            System.IO.StreamWriter SaveEventID = new System.IO.StreamWriter("EventID");
                                            SaveEventID.WriteLine(eventIdTextBox.Text);
                                            SaveEventID.Close();

                                        }

                                        rowdata = rowdata + " '" + dataGridView1.Rows[i].Cells[j].Value.ToString() + "'";

                                    }
                                    catch (Exception Ee)
                                    {
                                        LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + Ee.ToString());
                                        return false;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        rowdata = rowdata + " '" + dataGridView1.Rows[i].Cells[j].Value.ToString() + "'";
                                    }
                                    catch (Exception Ee)
                                    {
                                        LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + Ee.ToString());
                                        return false;
                                    }
                                }
                            }
                        }

                        
                    }
                    else
                    {
                        try
                        {
                            rowdata = rowdata + " null";
                        }
                        catch (Exception Ee)
                        {
                            LogListBox.Items.Insert(0,Ee.ToString());
                            return false;
                        }
                    } 
                                       
                }
                //koniec

                rowdata = "INSERT INTO " + tableNameTextBox.Text + "(`Action`, `Actor`, `AlarmLimit`, `AreaCode`, `AreaName`, `Block`, `Category`, `ChangedTime`, `Comment`, `ConditionName`, `Description`, `EventID`, `GDAServerName`, `LocationTagName`, `PrevValue`, `Priority`, `Source`, `SourceEntityName`, `Station`, `Time`, `TransactionID`, `Units`, `Value`) VALUES (" + rowdata + ")";



                    try
                    {
                        cmd.Connection = connectionSQL;
                        cmd.CommandText = rowdata;
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        writeCounter++;
                    }
                    catch (Exception ex)
                    {
                        if (connectionSQL.State == ConnectionState.Open)
                        {
                            connectionSQL.Close();
                            LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Koniec polaczenia MySQL- error1");
                        }
                        LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + ex.Message);
                        errorCounter++;
                    }
                

                rowdata = null;
            }

            //koniec petli do zapisywania

            LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- zapisano do MySQL:" + writeCounter +", bledow zapisu: " + errorCounter);

            reWriteItemsTextBox.Enabled = true;
            reWriteItemsTextBox.Text = writeCounter.ToString();
            reWriteItemsTextBox.Enabled = false;

            sumWriteTextBox.Enabled = true;
            sumWriteTextBox.Text = (int.Parse(sumWriteTextBox.Text) + int.Parse(reWriteItemsTextBox.Text)).ToString();
            sumWriteTextBox.Enabled = false;

            try
                {
                    cmd.Connection = connectionSQL;
                    cmd.CommandText = "COMMIT;";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (connectionSQL.State == ConnectionState.Open)
                    {
                        connectionSQL.Close();
                        LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Koniec polaczenia MySQL- error");
                        reWriteTimer.Stop();
                        reWriteTimer.Enabled = false;
                }
                    LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + ex.Message);
                }



            connectionSQL.Close();
            LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Koniec polaczenia MySQL- koniec petli");
            reWriteTimer.Stop();
            reWriteTimer.Enabled = false;
            return true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
        public static List<String> GetSystemDSNList()
        {
            // generowanie listy DSN ################################################
            List<string> names = new List<string>();
            // get system dsn's
  
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.CurrentUser).OpenSubKey("Software");
                                                
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC.INI");
                    if (reg != null)
                    {
                        // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                        foreach (string sName in reg.GetSubKeyNames())
                            {
                                names.Add(sName);
                            }
                    }
                    try
                    {
                        reg.Close();
                    }
                    catch { /* ignore this exception if we couldn't close */ }
                    
                }
            }


            return names;
        }

       

        private void connectButton_Load_1(object sender, EventArgs e)
        {
            //wczytanie pierwszego event ID
                string line;
           if (File.Exists("EventID"))
            {
                StreamReader sr = new StreamReader("EventID");

                while ((line = sr.ReadLine()) != null)
                {
                    eventIdTextBox.Text = line;
                }
                sr.Close();
            }
            else
            {
                eventIdTextBox.Text = "0";
            }
                

            LogListBox.Items.Insert(0,"Uruchomiono aplikacje.");
            tabPage1.Text = "Connection";
            tabPage2.Text = "Settings and log parameters";
            rewComboBox.Items.Add("R410");
            rewComboBox.Items.Add("R430");
            rewComboBox.Items.Add("R431");
            rewComboBox.SelectedItem = "R431";
            reWriteItemsTextBox.Enabled = false;
            sumWriteTextBox.Enabled = false;

            if (Environment.Is64BitOperatingSystem)
            {
                System.Windows.Forms.MessageBox.Show("It is 64bit operating system. Please check DSN data", "System Environment", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                comboDSN.DataSource = GetSystemDSNList();
            }
        }


        private void saveConfAs()
        {
            string myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.FileName) != null)
                {
                    System.IO.StreamWriter SaveConf = new System.IO.StreamWriter(myStream);
                    SaveConf.WriteLine(serverName.Text);
                    SaveConf.WriteLine(portNumericUpDown1.Text);
                    SaveConf.WriteLine(dbNameTextBox.Text);
                    SaveConf.WriteLine(tableNameTextBox.Text);
                    SaveConf.WriteLine(userTextBox.Text);
                    SaveConf.WriteLine(pwdTextBox.Text);
                    SaveConf.WriteLine(comboDSN.Text);
                    SaveConf.WriteLine(rewComboBox.Text);
                    SaveConf.WriteLine(eventNoNumericUpDown.Text);
                    SaveConf.WriteLine(uidTextBox.Text);
                    SaveConf.WriteLine(pswTextBox.Text);
                    SaveConf.Close();
                }
            }     
        }


        private void loadConf()
        {
            string myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = openFileDialog1.FileName) != null)
                {
                    System.IO.StreamReader LoadConf = new System.IO.StreamReader(myStream);
                    serverName.Text = LoadConf.ReadLine();
                    portNumericUpDown1.Text = LoadConf.ReadLine();
                    dbNameTextBox.Text = LoadConf.ReadLine();
                    tableNameTextBox.Text = LoadConf.ReadLine();
                    userTextBox.Text = LoadConf.ReadLine();
                    pwdTextBox.Text = LoadConf.ReadLine();
                    comboDSN.Text = LoadConf.ReadLine();
                    rewComboBox.Text = LoadConf.ReadLine();
                    eventNoNumericUpDown.Text = LoadConf.ReadLine();
                    uidTextBox.Text = LoadConf.ReadLine();
                    pswTextBox.Text = LoadConf.ReadLine();
                    LoadConf.Close();
                    // myStream.Close();
                }
            }
        }


        private void comboDSN_Click_1(object sender, EventArgs e)
        {
                comboDSN.DataSource = GetSystemDSNList();       
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            switch (rewComboBox.Text)
            {
                case "R410":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Integrated Security=SSPI", queryTextBox.Text);
                    break;
                case "R430":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Integrated Security=SSPI", queryTextBox.Text);
                    break;
                case "R431":
                    ReadDataTable("DSN=" + comboDSN.Text + "; Uid=" + uidTextBox.Text + "; Pwd=" + pwdTextBox.Text + ";", queryTextBox.Text);
                    break;
                default:
                    ReadDataTable("DSN=" + comboDSN.Text + "; Uid=" + uidTextBox.Text + "; Pwd=" + pwdTextBox.Text + ";", queryTextBox.Text);
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection connectionSQL;
            string myConnectionString;

            myConnectionString = "Server=" + serverName.Text + "; UID=" + userTextBox.Text + "; Pwd=" + pwdTextBox.Text + "; database=" + dbNameTextBox.Text + ";";

            connectionSQL = new MySql.Data.MySqlClient.MySqlConnection();
            connectionSQL.ConnectionString = myConnectionString;
            connectionSQL.Open();
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
            LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "Poczatek polaczenia MySQL");
            reWriteTimer.Enabled = true;
            reWriteTimer.Start();
            
            try
            {
                cmd.Connection = connectionSQL;
                cmd.CommandText = "INSERT INTO " + tableNameTextBox.Text + "(`Action`, `Actor`) VALUES (null,null)";
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (connectionSQL.State == ConnectionState.Open)
                {
                    connectionSQL.Close();
                    LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + "- Koniec polaczenia MySQL- error");
                    reWriteTimer.Stop();
                    reWriteTimer.Enabled = false;
                }
                LogListBox.Items.Insert(0,DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + ex.Message);
            }
            connectionSQL.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            reWriteTimer.Stop();
            reWriteTimer.Enabled = false;

            ReWrite();

            reWriteTimer.Enabled = true;
            reWriteTimer.Start();
        }


        private void timerNumericUpDown_ValueChanged_1(object sender, EventArgs e)
        {
            reWriteTimer.Interval = int.Parse(timerNumericUpDown.Value.ToString()) * 1000 * 60; //czas w min
        }

        private void triggerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(triggerCheckBox.Checked)
            {
                reWriteTimer.Interval = int.Parse(timerNumericUpDown.Value.ToString()) * 1000 * 60; //czas w min
                reWriteTimer.Enabled = true;
                reWriteTimer.Start();

                QueryButton.Enabled = false;
                insertButton.Enabled = false;
                reWriteButton.Enabled = false;
                odbcGroupBox.Enabled = false;
                mySqlGroupBox.Enabled = false;
                queryTextBox.Enabled = false;
            }
            else
            {
                reWriteTimer.Interval = int.Parse(timerNumericUpDown.Value.ToString()) * 1000 * 60; //czas w min
                reWriteTimer.Stop();
                reWriteTimer.Enabled = false;

                QueryButton.Enabled = true;
                insertButton.Enabled = true;
                reWriteButton.Enabled = true;
                odbcGroupBox.Enabled = true;
                mySqlGroupBox.Enabled = true;
                queryTextBox.Enabled = true;
            }
                
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (triggerCheckBox.Checked)
            {
                reWriteTimer.Stop();
                reWriteTimer.Enabled = false;

                ReWrite();
                        

                reWriteTimer.Enabled = true;
                reWriteTimer.Start();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LogListBox.Items.Clear();
        }

        public void saveLogFile()
        {
                System.IO.StreamWriter SaveFile = new System.IO.StreamWriter("ODBClog_" + DateTime.Now.ToString("yyyyMMdd_hh_mm"));
                foreach (var item in LogListBox.Items)
                {
                    SaveFile.WriteLine(item);
                }

                SaveFile.Close();
                LogListBox.Items.Clear();
        }

        private void timerLogClear_Tick(object sender, EventArgs e)
        {
            if (autoLogClear.Checked)
            {
                saveLogFile();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            LogListBox.Items.Insert(0,"test");
            LogListBox.Items.Clear();
        }

        private void autoLogClear_CheckedChanged_1(object sender, EventArgs e)
        {
            if (autoLogClear.Checked)
            {
                timeLogNumericUpDown.Enabled = true;
                timerLogClear.Interval = int.Parse(timeLogNumericUpDown.Value.ToString());
                timerLogClear.Enabled = true;
                timerLogClear.Start();
            }
            else
            {
                timerLogClear.Stop();
                timerLogClear.Enabled = false;
                timeLogNumericUpDown.Enabled = false;
            }
        }

        private void timeLogNumericUpDown_ValueChanged_1(object sender, EventArgs e)
        {
            timerLogClear.Interval = int.Parse(timeLogNumericUpDown.Value.ToString()) * 1000 * 60; //czas w min
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void rewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rewComboBox.Text)
            {
                case "R410":
                    uidTextBox.Enabled = false;
                    pswTextBox.Enabled = false;
                    break;
                case "R430":
                    uidTextBox.Enabled = false;
                    pswTextBox.Enabled = false;
                    break;
                case "R431":
                    uidTextBox.Enabled = true;
                    pswTextBox.Enabled = true;
                    break;
                default:
                    uidTextBox.Enabled = true;
                    pswTextBox.Enabled = true;
                    break;
            }
        }

        private void progresBarTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value < 100)
            {
                progressBar.Value = progressBar.Value + 5;
            }
            else
            {
                progressBar.Value = 0;
            }
        }


        private void button1_Click_2(object sender, EventArgs e)
        {
            LogListBox.Items.Insert(0,commentTextBox.Text);
        }

        private void saveLogButton_Click(object sender, EventArgs e)
        {
            saveLogFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveConfAs();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadConf();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }


    }
}
