using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SigknowShopFloor
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for StationB.xaml
    /// </summary>
    public partial class StationB : Window
    {
        Label lbStation = new Label();
        Label lbUser = new Label();
        Label lbUsertitle = new Label();
        StackPanel spUser = new StackPanel();
        Label lbPCBASN = new Label();
        Border bPCBASN = new Border();
        Border bRESULT = new Border();
        TextBox tbPCBASN = new TextBox();
        Label lbRESULT = new Label();
        TextBox tbRESULT = new TextBox();
        StackPanel spMainBody = new StackPanel();

        private MainWindow mainwindow;
        public StationB()
        {
            InitializeComponent();
            ConstructObjects();
            Closing += OnWindowClosing;
        }

        public StationB(MainWindow mw): this()
        {
            mainwindow = mw;
        }

        public void ConstructObjects()
        {
            //Label lbStation = new Label();
            lbStation.Content = StationName.gStationB;
            lbStation.FontSize = 30;

            //Label lbUser = new Label();
            lbUser.Content = Global.gUsername;
            lbUser.FontSize = 20;
            //Label lbUsertitle = new Label();
            lbUsertitle.Content = ShopFloorLabel.OperaterName;
            lbUsertitle.FontSize = 16;

            //StackPanel spUser = new StackPanel();
            spUser.Orientation = Orientation.Horizontal;
            spUser.HorizontalAlignment = HorizontalAlignment.Right;
            spUser.Children.Add(lbUsertitle);
            spUser.Children.Add(lbUser);

            //////////////////////////////////////////////
            //Label lbPCBASN = new Label();
            lbPCBASN.Content = ShopFloorLabel.PCBASN;

            //Border bPCBASN = new Border();
            bPCBASN.Width = 200;
            bPCBASN.Height = 30;
            bPCBASN.BorderBrush = System.Windows.Media.Brushes.Black;
            bPCBASN.BorderThickness = new Thickness(4);

            //TextBox tbPCBASN = new TextBox();
            tbPCBASN.Height = 30;
            tbPCBASN.Width = 200;
            tbPCBASN.Text = Global.gEMPTY;
            tbPCBASN.Background = System.Windows.Media.Brushes.LightGray;
            tbPCBASN.KeyDown += tbPCBASN_KeyDown;
            //tbPCBASN.MouseDown += tbPCBASN_MouseDown;
            bPCBASN.Child = tbPCBASN;

            //Label lbRESULT = new Label();
            lbRESULT.Content = ShopFloorLabel.RESULT;

            //Border bRESULT = new Border();
            bRESULT.Width = 200;
            bRESULT.Height = 30;
            bRESULT.BorderBrush = System.Windows.Media.Brushes.Black;
            bRESULT.BorderThickness = new Thickness(4);

            //TextBox tbRESULT = new TextBox();
            tbRESULT.Height = 30;
            tbRESULT.Width = 200;
            tbRESULT.Text = Global.gEMPTY;
            tbRESULT.Background = System.Windows.Media.Brushes.LightGray;
            tbRESULT.KeyDown += tbRESULT_KeyDown;
            bRESULT.Child = tbRESULT;

            //StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Center;
            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(spUser);
            spMainBody.Children.Add(lbPCBASN);
            spMainBody.Children.Add(bPCBASN);
            spMainBody.Children.Add(lbRESULT);
            spMainBody.Children.Add(bRESULT);
            Utils.changeTextboxLang2Eng(tbPCBASN);

            Content = spMainBody;
            tbPCBASN.Focus();
            tbPCBASN.SelectAll();

        }
        
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            mainwindow.Visibility = Visibility.Visible;
        }

        private void tbPCBASN_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                if (textBox.Text != "")
                {
                    Global.gPCBASN = textBox.Text;
                }
                else
                    return;
                try
                {
                    tbRESULT.Background = System.Windows.Media.Brushes.LightGray;
                    Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationA);
                    tbRESULT.Focus();
                    tbRESULT.Clear();
                }
                catch (InvalidSerialNumberException isne)
                {
                    tbPCBASN.Clear();
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("PCBA序號 '" + Global.gPCBASN + "' 格式不符合規定.");
                }
                catch (PreviousErrorException pe)
                {
                    tbPCBASN.Clear();
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("前一站測試未通過.");
                }
                catch (Exception ex)
                {
                    tbPCBASN.Clear();
                    tbRESULT.Clear();
                    MessageBox.Show("PCBASN 輸入發生錯誤");
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                }

            }

        }

        private void tbRESULT_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Foreground = System.Windows.Media.Brushes.Black;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                if (textBox.Text != "")
                {
                    Global.gResult = String.Copy(textBox.Text);
                }
                else
                    return;
                try
                {
                    tbPCBASN.Background = System.Windows.Media.Brushes.LightGray;
                    Utils.ValidateResult(Global.gResult);
                    Utils.ValidateResult(DBColPrefix.gStationB, tbPCBASN.Text, Global.gResult);
                    textBox.Background = System.Windows.Media.Brushes.Yellow;
                    if (Global.gResult == Barcode.OK)
                    {
                        textBox.Text = Global.OK;
                        textBox.Background = System.Windows.Media.Brushes.LightGreen;
                        textBox.Foreground = System.Windows.Media.Brushes.Black;
                        if (Global.gREWORK)
                        {
                            if (!Global.gSKIP)
                            {
                                Utils.dbupdate(tbPCBASN.Text, DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                                if (!Global.gINITIALRUN)
                                    Utils.dbchangehistory(tbPCBASN.Text, "", DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                            }
                        }
                        else
                        {
                            //throw new Exception("未按照標準程序 : 查無前一站資料.");
                            Utils.dbinsert(tbPCBASN.Text, DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                        }
                        tbPCBASN.Focus();
                        tbPCBASN.SelectAll();
                    }
                    else if (Global.gResult == Barcode.NG)
                    {
                        textBox.Text = Global.NG;
                        textBox.Background = System.Windows.Media.Brushes.OrangeRed;
                        textBox.Foreground = System.Windows.Media.Brushes.Black;
                        if (Global.gREWORK)
                        {
                            if (!Global.gSKIP)
                            {
                                Utils.dbupdate(tbPCBASN.Text, DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                                if (!Global.gINITIALRUN)
                                    Utils.dbchangehistory(tbPCBASN.Text, "", DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                            }
                        }
                        else
                        {
                            //throw new Exception("未按照標準程序 : 查無前一站資料.");
                            Utils.dbinsert(tbPCBASN.Text, DBColPrefix.gStationB, Utils.barcode2dbbool(Global.gResult));
                        }
                        tbPCBASN.Focus();
                        tbPCBASN.SelectAll();
                    }
                    else
                    {
                        textBox.Foreground = System.Windows.Media.Brushes.Red;
                        tbRESULT.Focus();
                        tbRESULT.Clear();
                        Utils.ErrorBeep();
                    }
                }
                catch (InvalidSerialNumberException isne)
                {
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("測試結果條碼格式不符合規定. 請掃瞄 OK 或 NG.");
                }
                catch (Exception ex)
                {
                    tbRESULT.Focus();
                    tbRESULT.Clear();
                    MessageBox.Show(ex.ToString());
                }

            }
        }
    }
}