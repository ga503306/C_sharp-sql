using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static string dbHost = "localhost";
        static string dbUser = "root";
        static string dbPass = "";
        static string dbName = "userregisterclassroom";
        static string sqldata;

        public Form1()
        {
            InitializeComponent();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            

        }

        private delegate void myUICallBack(string myStr, Control ctl); //跨執行續
        private void myUI(string myStr, Control ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack myUpdate = new myUICallBack(myUI);
                this.Invoke(myUpdate, myStr, ctl);
            }
            else
            {
                ctl.Text = myStr;
            }
        }

        string sql(string rfid)
        {
            //string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName + ";CharSet=utf8_general_ci;";
            string strMySqlServer = String.Format("server=" + dbHost + ";user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + ";");
            MySqlConnection conn = new MySqlConnection(strMySqlServer);

            // 連線到資料庫 
            try
            {
                conn.Open();
                // 進行select
                string SQL = "SELECT * FROM  `registerclassroom` WHERE  `userid` = "+ rfid;
                //string SQL = "SELECT userid,data,classroom FROM registerclassroom";
                //  string SQL = "SELECT userid FROM idpass";
                //string SQL = "SELECT rfid.rfid FROM `rfid` where rfid = '03D9E6C7'";
                MessageBox.Show(SQL);

                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    MySqlDataReader myData = cmd.ExecuteReader();

                    if (!myData.HasRows)
                    {
                        // 如果沒有資料,顯示沒有資料的訊息 
                        MessageBox.Show("No data.");
                        return "0";
                    }
                    else
                    {
                        // 讀取資料並且顯示出來 
                        while (myData.Read())
                        {
                            //label1.Text = myData.GetString(0) + " " ;
                            //label2.Text = myData.GetString(1) + " " + myData.GetString(2) + " " + myData.GetString(3);
                            MessageBox.Show(myData.GetString(0) + myData.GetString(1) + myData.GetString(2), "資料");
                            //return true;
                        }
                        myData.Close();
                        return myData.GetString(0);
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show("Error " + ex.Number + " : " + ex.Message);
                    return "0";
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("無法連線到資料庫.");
                        break;
                    case 1042:
                        MessageBox.Show("無效的主機名稱");
                        break;
                    case 1045:
                        MessageBox.Show("使用者帳號或密碼錯誤,請再試一次.");
                        break;
                }
                return "0";
            }
        }
       
        

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] serialPorts = SerialPort.GetPortNames();

            foreach (string serialPort in serialPorts)
            {
                cbCOM.Items.Add(serialPort);
                if (cbCOM.Items.Count > 0)
                {
                    cbCOM.SelectedIndex = 0;
                }
            }
            
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            sqlsearch_all("SELECT * FROM  `registerclassroom` LIMIT 0 , 30");

        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = (string)cbCOM.SelectedItem;
            serialPort1.Open();
            label2.Text = "connting to " + cbCOM.SelectedItem.ToString();
            label2.ForeColor = Color.Green;

        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            serialPort1.Write("1");

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            label2.Text = "disconnting";
            label2.ForeColor = Color.Red;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        void serialPort1_DataReceived(object sender,  EventArgs e)
        {
            string data = serialPort1.ReadLine();

            //label1.Text = "datareceived: " + data;
           
            data = data.Replace("\r", "").Replace("\n", "");
            myUI("datareceived: " + data, Username);
            //data=data.Replace(@"\r\n", ""); 
            sqldata = sql(data);


            if (data.Equals(sqldata))
            {
                serialPort1.Write("1");
                MessageBox.Show("開鎖", data);
            }
            else{
                serialPort1.Write("2");
                MessageBox.Show("滾", data);
            }
            
         }



        string sqlsearch_all(string search)
        {
            //string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName + ";CharSet=utf8_general_ci;";
            string strMySqlServer = String.Format("server=" + dbHost + ";user id=" + dbUser + "; password=" + dbPass + "; database=" + dbName + ";");
            MySqlConnection conn = new MySqlConnection(strMySqlServer);

            // 連線到資料庫 
            try
            {
                conn.Open();
                // 進行select
                string SQL = search;
                //string SQL = "SELECT userid,data,classroom FROM registerclassroom";
                //  string SQL = "SELECT userid FROM idpass";
                //string SQL = "SELECT rfid.rfid FROM `rfid` where rfid = '03D9E6C7'";
                MessageBox.Show(SQL);

                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    MySqlDataReader myData = cmd.ExecuteReader();

                    if (!myData.HasRows)
                    {
                        // 如果沒有資料,顯示沒有資料的訊息 
                        MessageBox.Show("No data.");
                        return "0";
                    }
                    else
                    {
                        // 讀取資料並且顯示出來 
                        while (myData.Read())
                        {

                            sqldata_all.Items.Add(myData.GetString(0) + "  " + myData.GetString(1) + " " +myData.GetString(2));
                       
                            //return true;
                        }
                        myData.Close();
                        return myData.GetString(0);
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show("Error " + ex.Number + " : " + ex.Message);
                    return "0";
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("無法連線到資料庫.");
                        break;
                    case 1042:
                        MessageBox.Show("無效的主機名稱");
                        break;
                    case 1045:
                        MessageBox.Show("使用者帳號或密碼錯誤,請再試一次.");
                        break;
                }
                return "0";
            }
        }
       
        

    }
}
