using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SigknowShopFloor
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for StationF.xaml
    /// </summary>
    public partial class StationF : Window
    {
        Label lbStation = new Label();
        Label lbUser = new Label();
        Label lbUsertitle = new Label();
        StackPanel spUser = new StackPanel();
        Label lbSIGKNOWSN = new Label();
        Border bSIGKNOWSN = new Border();
        Border bRESULT = new Border();
        TextBox tbSIGKNOWSN = new TextBox();
        Label lbRESULT = new Label();
        TextBox tbRESULT = new TextBox();
        StackPanel spMainBody = new StackPanel();

        private MainWindow mainwindow;
        public StationF()
        {
            InitializeComponent();
            ConstructObjects();
            Closing += OnWindowClosing;
        }

        public StationF(MainWindow mw)
            : this()
        {
            mainwindow = mw;
        }

        public void ConstructObjects()
        {
            //Label lbStation = new Label();
            lbStation.Content = StationName.gStationF;
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
            //Label lbSIGKNOWSN = new Label();
            lbSIGKNOWSN.Content = ShopFloorLabel.SIGKNOWSN;

            //Border bSIGKNOWSN = new Border();
            bSIGKNOWSN.Width = 200;
            bSIGKNOWSN.Height = 30;
            bSIGKNOWSN.BorderBrush = System.Windows.Media.Brushes.Black;
            bSIGKNOWSN.BorderThickness = new Thickness(4);

            //TextBox tbSIGKNOWSN = new TextBox();
            tbSIGKNOWSN.Height = 30;
            tbSIGKNOWSN.Width = 200;
            tbSIGKNOWSN.Text = Global.gEMPTY;
            tbSIGKNOWSN.Background = System.Windows.Media.Brushes.LightGray;
            tbSIGKNOWSN.KeyDown += tbSIGKNOWSN_KeyDown;
            //tbSIGKNOWSN.MouseDown += tbSIGKNOWSN_MouseDown;
            bSIGKNOWSN.Child = tbSIGKNOWSN;

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
            spMainBody.Children.Add(lbSIGKNOWSN);
            spMainBody.Children.Add(bSIGKNOWSN);
            spMainBody.Children.Add(lbRESULT);
            spMainBody.Children.Add(bRESULT);
            Utils.changeTextboxLang2Eng(tbSIGKNOWSN);

            Content = spMainBody;
            tbSIGKNOWSN.Focus();
            tbSIGKNOWSN.SelectAll();

        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            mainwindow.Visibility = Visibility.Visible;
        }

        private void tbSIGKNOWSN_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                if (textBox.Text != "")
                {
                    Global.gSIGKNOWSN = textBox.Text;
                }
                else
                    return;
                try
                {
                    tbRESULT.Background = System.Windows.Media.Brushes.LightGray;
                    Global.gPCBASN = SNAssociate.GetPCBASN(Global.gSIGKNOWSN);
                    //Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationD); // Eresult is skipped.
                    Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationB, DBColPrefix.gStationC); // Eresult is skipped.
                    tbRESULT.Focus();
                    tbRESULT.Clear();
                }
                catch (InvalidSerialNumberException isne)
                {
                    tbSIGKNOWSN.Clear();
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("上蓋序號 '" + Global.gSIGKNOWSN + "' 格式不符合規定.");
                }
                catch (PreviousErrorException pe)
                {
                    tbSIGKNOWSN.Clear();
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("前一站測試未通過.");
                }
                catch (SerialNumberNotMatchedException snnm)
                {
                    tbSIGKNOWSN.Clear();
                    tbRESULT.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("上蓋序號'" + Global.gSIGKNOWSN + "' 查無 PCBA 關聯.");

                }
                catch (Exception ex)
                {
                    tbSIGKNOWSN.Clear();
                    tbRESULT.Clear();
                    MessageBox.Show("SIGKNOWSN 輸入發生錯誤");
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                    tbSIGKNOWSN.Focus();
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
                try
                {
                    if (String.Compare(textBox.Text, "") == 0) return;

                    Global.gResult = String.Copy(textBox.Text);

                    if ((String.Compare(textBox.Text, Barcode.OK) == 0)
                        || (String.Compare(textBox.Text, Barcode.NG) == 0))
                    {
                        // do nothing
                    }
                    else if (String.Compare(textBox.Text, Global.gSIGKNOWSN) == 0)
                    {
                        // do nothing
                        tbRESULT.Clear();
                        tbRESULT.Focus();
                        return;
                    }
                    else
                    {
                        throw new SerialNumberNotMatchedException();
                    }

                    tbSIGKNOWSN.Background = System.Windows.Media.Brushes.LightGray;
                    //Utils.ValidateResult(Global.gResult);
                    Utils.ValidateResult(DBColPrefix.gStationF, Global.gPCBASN, Global.gResult);
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
                                Utils.dbupdate(
                                    Global.gPCBASN,
                                    DBColPrefix.gStationF,
                                    Utils.barcode2dbbool(Global.gResult));
                                if (!Global.gINITIALRUN)
                                    Utils.dbchangehistory(
                                        Global.gPCBASN,
                                        "",
                                        DBColPrefix.gStationF,
                                        Utils.barcode2dbbool(Global.gResult));
                            }
                        }
                        else
                        {
                            throw new Exception("未按照標準程序 : 查無前一站資料.");
                        }
                        tbSIGKNOWSN.Focus();
                        tbSIGKNOWSN.SelectAll();
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
                                Utils.dbupdate(
                                    Global.gPCBASN,
                                    DBColPrefix.gStationF,
                                    Utils.barcode2dbbool(Global.gResult));
                                if (!Global.gINITIALRUN)
                                    Utils.dbchangehistory(
                                        Global.gPCBASN,
                                        "",
                                        DBColPrefix.gStationF,
                                        Utils.barcode2dbbool(Global.gResult));
                            }
                        }
                        else
                        {
                            throw new Exception("序號尚未存在系統");
                        }
                        tbSIGKNOWSN.Focus();
                        tbSIGKNOWSN.SelectAll();
                    }
                    else
                    {
                        textBox.Foreground = System.Windows.Media.Brushes.Red;
                        tbRESULT.Focus();
                        tbRESULT.Clear();
                        Utils.ErrorBeep();
                    }
                }
                catch (SerialNumberNotMatchedException sx)
                {
                    MessageBox.Show("標籤'" + textBox.Text + "' 與上蓋序號不一致.");
                    tbRESULT.Focus();
                    tbRESULT.Clear();
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
