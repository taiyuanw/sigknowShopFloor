using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SigknowShopFloor
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for StationG.xaml
    /// </summary>
    public partial class StationG : Window
    {
        Label lbStation = new Label();
        Label lbUser = new Label();
        Label lbUsertitle = new Label();
        StackPanel spUser = new StackPanel();
        Label lbSIGKNOWSN = new Label();
        Border bSIGKNOWSN = new Border();
        Border bBOXSN = new Border();
        TextBox tbSIGKNOWSN = new TextBox();
        Label lbBOXSN = new Label();
        TextBox tbBOXSN = new TextBox();
        StackPanel spMainBody = new StackPanel();

        private MainWindow mainwindow;
        public StationG()
        {
            InitializeComponent();
            ConstructObjects();
            Closing += OnWindowClosing;
        }

        public StationG(MainWindow mw)
            : this()
        {
            mainwindow = mw;
        }

        public void ConstructObjects()
        {
            //Label lbStation = new Label();
            lbStation.Content = StationName.gStationG;
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
            lbBOXSN.Content = ShopFloorLabel.BOXSN;

            //Border bRESULT = new Border();
            bBOXSN.Width = 200;
            bBOXSN.Height = 30;
            bBOXSN.BorderBrush = System.Windows.Media.Brushes.Black;
            bBOXSN.BorderThickness = new Thickness(4);

            //TextBox tbRESULT = new TextBox();
            tbBOXSN.Height = 30;
            tbBOXSN.Width = 200;
            tbBOXSN.Text = Global.gEMPTY;
            tbBOXSN.Background = System.Windows.Media.Brushes.LightGray;
            tbBOXSN.KeyDown += tbBOXSN_KeyDown;
            bBOXSN.Child = tbBOXSN;

            //StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Center;
            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(spUser);
            spMainBody.Children.Add(lbSIGKNOWSN);
            spMainBody.Children.Add(bSIGKNOWSN);
            spMainBody.Children.Add(lbBOXSN);
            spMainBody.Children.Add(bBOXSN);
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
                    tbBOXSN.Background = System.Windows.Media.Brushes.LightGray;
                    Global.gPCBASN = SNAssociate.GetPCBASN(Global.gSIGKNOWSN);
                    //Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationD, DBColPrefix.gStationF);
                    Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationF);
                    tbBOXSN.Focus();
                    tbBOXSN.Clear();
                }
                catch (InvalidSerialNumberException isne)
                {
                    tbSIGKNOWSN.Clear();
                    tbBOXSN.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("上蓋序號 '" + Global.gSIGKNOWSN + "' 格式不符合規定.");
                }
                catch (PreviousErrorException pe)
                {
                    tbSIGKNOWSN.Clear();
                    tbBOXSN.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("前一站測試未通過.");
                }
                catch (SerialNumberNotMatchedException snnm)
                {
                    tbSIGKNOWSN.Clear();
                    tbBOXSN.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("上蓋序號'" + Global.gSIGKNOWSN + "' 查無 PCBA 關聯.");

                }
                catch (Exception ex)
                {
                    tbSIGKNOWSN.Clear();
                    tbBOXSN.Clear();
                    MessageBox.Show("SIGKNOWSN 輸入發生錯誤");
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                    tbSIGKNOWSN.Focus();
                }

            }

        }

        private void tbBOXSN_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Foreground = System.Windows.Media.Brushes.Black;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                try
                {
                    if (String.Compare(textBox.Text, "") == 0) return;

                    Global.gBOXSN = String.Copy(textBox.Text);


                    tbSIGKNOWSN.Background = System.Windows.Media.Brushes.LightGray;
                    Utils.ValidateBoxSN(DBColPrefix.gStationG, Global.gPCBASN, Global.gBOXSN);



                    textBox.Background = System.Windows.Media.Brushes.Yellow;


                    if (Global.gREWORK)
                    {
                        if (!Global.gSKIP)
                        {
                            Boxing.dbupdate(Global.gPCBASN, Global.gBOXSN);
                            if (!Global.gINITIALRUN)
                                Boxing.dbchangehistory(
                                    Global.gPCBASN,
                                    Global.gSIGKNOWSN,
                                    DBColPrefix.gStationG,
                                    Global.gBOXSN);
                        }
                    }
                    else
                    {
                        throw new Exception("未按照標準程序 : 查無前一站資料.");
                    }

                }
                catch (ResultUnchangedException rx)
                {
                    // do nothing
                }
                catch (SerialNumberNotMatchedException sx)
                {
                    MessageBox.Show("序號'" + textBox.Text + "' 與上蓋序號不一致.");
                    tbBOXSN.Focus();
                    tbBOXSN.Clear();
                }
                catch (Exception ex)
                {
                    tbBOXSN.Focus();
                    tbBOXSN.Clear();
                    MessageBox.Show(ex.ToString());
                }
                tbSIGKNOWSN.Clear();
                tbBOXSN.Clear();
                tbSIGKNOWSN.Focus();
            }
        }
    }
}
