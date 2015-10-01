using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
        private List<string> lstPATCHSN = new List<string>();

        private Label lbBoxContent = new Label();
        private StackPanel spPatchMatrix = new StackPanel();
        private StackPanel spCol0 = new StackPanel();
        private StackPanel spCol1 = new StackPanel();
        private StackPanel spCol2 = new StackPanel();
        private StackPanel spCol3 = new StackPanel();
        private StackPanel spCol4 = new StackPanel();
        private StackPanel spCol5 = new StackPanel();

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
            bSIGKNOWSN.HorizontalAlignment = HorizontalAlignment.Left;
            bSIGKNOWSN.BorderBrush = System.Windows.Media.Brushes.Black;
            bSIGKNOWSN.BorderThickness = new Thickness(4);

            //TextBox tbSIGKNOWSN = new TextBox();
            tbSIGKNOWSN.Height = 30;
            tbSIGKNOWSN.Width = 200;
            tbSIGKNOWSN.HorizontalAlignment = HorizontalAlignment.Right;
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
            bBOXSN.HorizontalAlignment = HorizontalAlignment.Left;
            bBOXSN.BorderBrush = System.Windows.Media.Brushes.Black;
            bBOXSN.BorderThickness = new Thickness(4);

            //TextBox tbRESULT = new TextBox();
            tbBOXSN.Height = 30;
            tbBOXSN.Width = 200;
            tbBOXSN.Text = Global.gEMPTY;
            tbBOXSN.HorizontalAlignment = HorizontalAlignment.Right;
            tbBOXSN.Background = System.Windows.Media.Brushes.LightGray;
            tbBOXSN.KeyDown += tbBOXSN_KeyDown;
            bBOXSN.Child = tbBOXSN;

            // patch matrix

            int col = 0;
            int row = 0;
            for (col = 0; col < 6; col ++)
            {
                for (row = 0; row < 10; row ++)
                {
                    
                }
            }

            lbBoxContent.Height = 50;
            lbBoxContent.Width = 300;
            lbBoxContent.Content = "箱內容物 (點擊移除) : ";
            spPatchMatrix.Orientation = Orientation.Horizontal;
            spPatchMatrix.HorizontalAlignment = HorizontalAlignment.Right;
            spCol0.Width = 150;
            spCol0.Height = 500;
            spCol1.Width = 150;
            spCol1.Height = 500;
            spCol2.Width = 150;
            spCol2.Height = 500;
            spCol3.Width = 150;
            spCol3.Height = 500;
            spCol4.Width = 150;
            spCol4.Height = 500;
            spCol5.Width = 150;
            spCol5.Height = 500;
            spPatchMatrix.Children.Add(spCol0);
            spPatchMatrix.Children.Add(spCol1);
            spPatchMatrix.Children.Add(spCol2);
            spPatchMatrix.Children.Add(spCol3);
            spPatchMatrix.Children.Add(spCol4);
            spPatchMatrix.Children.Add(spCol5);
            
            //StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Center;
            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(spUser);
            spMainBody.Children.Add(lbBOXSN);
            spMainBody.Children.Add(bBOXSN);
            spMainBody.Children.Add(lbSIGKNOWSN);
            spMainBody.Children.Add(bSIGKNOWSN);
            spMainBody.Children.Add(lbBoxContent);
            spMainBody.Children.Add(spPatchMatrix);
            Utils.changeTextboxLang2Eng(tbSIGKNOWSN);

            Content = spMainBody;
            tbBOXSN.Focus();
            tbBOXSN.SelectAll();

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
                    Utils.ValidateSN(Global.gPCBASN, DBColPrefix.gStationB, DBColPrefix.gStationC, DBColPrefix.gStationF);
                    Utils.ValidateResultBoxing(Global.gBOXSN, Global.gSIGKNOWSN);


                    textBox.Background = System.Windows.Media.Brushes.LightGreen;
                    textBox.Foreground = System.Windows.Media.Brushes.Black;
                    if (Global.gREWORK)
                    {
                        if (!Global.gSKIP)
                        {
                            Boxing.dbupdate(Global.gPCBASN, Global.gBOXSN);
                            if (!Global.gINITIALRUN)
                                Utils.dbchangehistory(Global.gPCBASN, Global.gSIGKNOWSN, DBColPrefix.gStationG, Global.gBOXSN);
                        }
                    }
                    else
                    {
                        //throw new Exception("未按照標準程序 : 查無前一站資料.");
                        Utils.dbinsert(Global.gPCBASN, DBColPrefix.gStationG, Global.gBOXSN);
                    }
                    
                    //Boxing.dbupdate(Global.gPCBASN, Global.gBOXSN);
                    this.ShowBoxContent();
                    Utils.OKBeep();
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }
                catch (InvalidSerialNumberException isne)
                {
                    Utils.ErrorBeep();
                    MessageBox.Show("序號 '" + Global.gSIGKNOWSN + "' 格式不符合規定.");
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }
                catch (PreviousErrorException pe)
                {
                    Utils.ErrorBeep();
                    MessageBox.Show("序號 '"+ tbSIGKNOWSN.Text + "' 前一站測試未通過.");
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }
                catch (SerialNumberNotMatchedException snnm)
                {
                    Utils.ErrorBeep();
                    MessageBox.Show("上蓋序號'" + Global.gSIGKNOWSN + "' 查無 PCBA 關聯.");
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }
                catch (Exception ex)
                {
                    Utils.ErrorBeep();
                    MessageBox.Show("SIGKNOWSN 輸入時發生錯誤");
                    MessageBox.Show(ex.ToString());
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }

            }

        }

        private void ShowBoxContent()
        {
            //spPatchMatrix.Children.Clear(); 
            lstPATCHSN = Utils.GetPatchSNbyBoxSN(tbBOXSN.Text);
            spCol0.Children.Clear();
            spCol1.Children.Clear();
            spCol2.Children.Clear();
            spCol3.Children.Clear();
            spCol4.Children.Clear();
            spCol5.Children.Clear();

            for (var i = 0; i < lstPATCHSN.Count; i++)
            {
                Label lb = new Label();
                lb.Height = 30;
                lb.Width = 150;
                lb.Content = lstPATCHSN.ElementAt(i);
                Button x = new Button();
                x.Height = 30;
                x.Width = 150;
                x.Content = lstPATCHSN.ElementAt(i);
                x.Click += new RoutedEventHandler(ButtonXClickHandler());

                if (i < 10)
                    spCol0.Children.Add(x);
                else if ((i >= 10) && (i < 20))
                    spCol1.Children.Add(x);
                else if ((i >= 20) && (i < 30))
                    spCol2.Children.Add(x);
                else if ((i >= 30) && (i < 40))
                    spCol3.Children.Add(x);
                else if ((i >= 40) && (i < 50))
                    spCol4.Children.Add(x);
                else if (i >= 50)
                    spCol5.Children.Add(x);
            }

        }

        private Action<object, RoutedEventArgs> ButtonXClickHandler()
        {
            return (object sender, RoutedEventArgs e) =>
            {
                // Put your code here, it will be called when
                // the button is clicked
                Button b = sender as Button;
                //MessageBox.Show("content: " + b.Content + "\nboxsn: " + Global.gBOXSN);
                try
                {
                    Boxing.RemovePatchFromBox(Global.gBOXSN, b.Content.ToString());
                    ShowBoxContent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    throw;
                }
            };
        }

        private void tbBOXSN_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Foreground = System.Windows.Media.Brushes.Black;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                tbBOXSN.Text = textBox.Text;
                try
                { 
                    Utils.ValidateBoxSN(textBox.Text);
                    Global.gBOXSN = textBox.Text;
                    this.ShowBoxContent();
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
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
                catch (InvalidSerialNumberException ise)
                {
                    MessageBox.Show("無效的序號");
                    tbBOXSN.Focus();
                    tbBOXSN.Clear();
                }

                catch (Exception ex)
                {
                    tbBOXSN.Focus();
                    tbBOXSN.Clear();
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
