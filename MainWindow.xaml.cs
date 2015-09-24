using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace SigknowShopFloor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConstructObjects();

        }

        private void ConstructObjects()
        {
            var lbStation = new Label();
            lbStation.Content = "選擇站別 ( * ) :";

            var bStation = new Border();
            bStation.Width = 300;
            bStation.Height = 40;
            bStation.Background = Brushes.White;
            bStation.BorderBrush = Brushes.White;
            bStation.BorderThickness = new Thickness(5);

            var cbstation = new ComboBox();
            cbstation.Items.Add(StationName.gStationA);
            cbstation.Items.Add(StationName.gStationB);
            cbstation.Items.Add(StationName.gStationC);
            //cbstation.Items.Add(StationName.gStationD);
            cbstation.Items.Add(StationName.gStationE);
            cbstation.Items.Add(StationName.gStationF);
            cbstation.Items.Add(StationName.gStationG);

            bStation.Child = cbstation;
            cbstation.SelectionChanged += cbstation_SelectionChanged;

            var spName = new StackPanel();
            spName.HorizontalAlignment = HorizontalAlignment.Left;
            var lbUsername = new Label();
            lbUsername.Content = "操作人員  ( * ) :";
            var txtboxUsername = new TextBox();
            txtboxUsername.Height = 30;
            txtboxUsername.Width = 150;
            txtboxUsername.KeyUp += txtboxUsername_KeyUp;
            spName.Children.Add(lbUsername);
            spName.Children.Add(txtboxUsername);

            var lbLogin = new Label();
            lbLogin.Content = SigknowDBServer.gServer;

            var buttonLogin = new Button();
            buttonLogin.Content = "登入";
            buttonLogin.Click += buttonLogin_Click;
            buttonLogin.Height = 30;
            buttonLogin.Width = 50;

            var spServer = new StackPanel();
            spServer.HorizontalAlignment = HorizontalAlignment.Left;
            var lbServer = new Label();
            lbServer.Content = "資料庫網址  ( * ) :";
            var txtboxServer = new TextBox();
            txtboxServer.Height = 30;
            txtboxServer.Width = 150;
            txtboxServer.KeyUp += txtboxServer_KeyUp;
            var buttonServer = new Button();
            buttonServer.Content = "設定";
            buttonServer.Click += buttonServer_Click;
            buttonServer.Height = 30;
            buttonServer.Width = 50;
            Utils.changeTextboxLang2Eng(txtboxServer);
            var lbReport = new Label();
            lbReport.Content = "";
            spServer.Children.Add(lbServer);
            spServer.Children.Add(txtboxServer);

            var buttonReport = new Button();
            buttonReport.Content = "匯出資料";
            buttonReport.Click += buttonReport_Click;
            buttonReport.Height = 30;
            buttonReport.Width = 100;

            StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Left;

            spMainBody.Children.Add(spServer);
            spMainBody.Children.Add(buttonServer);

            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(bStation);
            spMainBody.Children.Add(spName);
            spMainBody.Children.Add(lbLogin);
            spMainBody.Children.Add(buttonLogin);

            
            spMainBody.Children.Add(lbReport);
            spMainBody.Children.Add(buttonReport);
            

            Content = spMainBody;

        }
        private void cbstation_SelectionChanged(object sender, EventArgs e)
        {
            var temp = sender as ComboBox;
            Global.gStation = temp.SelectedItem.ToString();
        }

        private void txtboxUsername_KeyUp(object sender, KeyEventArgs e)
        {
            var temp = sender as TextBox;
            Global.gUsername = temp.Text.Trim();
        }

        private void txtboxServer_KeyUp(object sender, KeyEventArgs e)
        {
            var temp = sender as TextBox;
            SigknowDBServer.gServer = temp.Text.Trim();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (SigknowDBServer.gServer == Global.gEMPTY)
            {
                MessageBox.Show("請先設定伺服器資料.");
                return;
            }
            if (Global.gStation == Global.gEMPTY)
            {
                MessageBox.Show("輸入站名");
                return;
            }
            if ((Global.gUsername == Global.gEMPTY) || (Global.gUsername == ""))
            {
                MessageBox.Show("輸入姓名");
                return;
            }
            switch (Global.gStation)
            {
                case StationName.gStationA:
                    var stationA = new StationA(this);
                    this.Hide();
                    stationA.ShowDialog();
                    break;
                case StationName.gStationB:
                    var stationB = new StationB(this);
                    this.Hide();
                    stationB.ShowDialog();
                    break;
                case StationName.gStationC:
                    var stationC = new StationC(this);
                    this.Hide();
                    stationC.ShowDialog();
                    break;
                //case StationName.gStationD:
                //    var stationD = new StationD(this);
                //    this.Hide();
                //    stationD.ShowDialog();
                //    break;
                case StationName.gStationE:
                    var stationE = new StationE(this);
                    this.Hide();
                    stationE.ShowDialog();
                    break;
                case StationName.gStationF:
                    var stationF = new StationF(this);
                    this.Hide();
                    stationF.ShowDialog();
                    break;
                case StationName.gStationG:
                    var stationG = new StationG(this);
                    this.Hide();
                    stationG.ShowDialog();
                    break;

            }


        }
        private void buttonServer_Click(object sender, RoutedEventArgs e)
        {
            if (SigknowDBServer.gServer == Global.gEMPTY)
            {
                MessageBox.Show("輸入 MySQL 伺服器 網址.");
            }
            else
            {
                try
                {
                    var cmd = "select count(*) from " + Global.gTableName + ";";
                    MySQLDB.DBconnect();
                    MySqlCommand sqlcmd = MySQLDB.command(cmd);
                    MySqlDataReader reader = sqlcmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read()) //一次讀一列 (row)
                        {
                        }
                    }
                    else
                    {
                        throw new DatabaseException();
                    }
                    MySQLDB.DBdisconnect();

                }
                catch (DatabaseException de)
                {
                    MessageBox.Show("設定 MySQL 伺服器 輸入發生錯誤.");
                    Utils.ErrorBeep();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("設定 MySQL 伺服器 輸入發生錯誤.");
                    Utils.ErrorBeep();
                    return;
                }

                MessageBox.Show("MySQL 伺服器 :" + SigknowDBServer.gServer + " 已設定.");
            }
            return;
            
        }
        private void buttonReport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            // Configure save file dialog box
            dlg.FileName = "ug101"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Comma Seperated Files |*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                DatabaseToFile.ExportShopFloor(dlg.FileName);
                //MessageBox.Show(dlg.FileName);
            }
        }
    }
}
