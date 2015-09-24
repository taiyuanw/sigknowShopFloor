using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SigknowShopFloor
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for StationEassociate.xaml
    /// </summary>
    public partial class StationEassociate : Window
    {
        Label lbStation = new Label();
        Label lbUser = new Label();
        Label lbUsertitle = new Label();
        StackPanel spUser = new StackPanel();

        Border bPCBASN = new Border();
        Label lbPCBASN = new Label();
        TextBox tbPCBASN = new TextBox();

        Border bSIGKNOWSN = new Border();
        Label lbSIGKNOWSN = new Label();
        TextBox tbSIGKNOWSN = new TextBox();

        Border bRESULT = new Border();
        Label lbRESULT = new Label();

        StackPanel spMainBody = new StackPanel();

        private StationE mainwindow;
        public StationEassociate()
        {
            InitializeComponent();
            ConstructObjects();
            Closing += OnWindowClosing;
        }

        public StationEassociate(StationE w) : this()
        {
            mainwindow = w;
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            mainwindow.Visibility = Visibility.Visible;
        }

        private void ConstructObjects()
        {
            //Label lbStation = new Label();
            lbStation.Content = StationName.gStationEassociation;
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
            //Label lbSelectAction = new Label();
            lbPCBASN.Content = ShopFloorLabel.PCBASN;

            //Border bSelectAction = new Border();
            bPCBASN.Width = 200;
            bPCBASN.Height = 30;
            bPCBASN.BorderBrush = System.Windows.Media.Brushes.Black;
            bPCBASN.BorderThickness = new Thickness(4);

            //TextBox tbSelectAction = new TextBox();
            tbPCBASN.Height = 30;
            tbPCBASN.Width = 200;
            tbPCBASN.Text = Global.gEMPTY;
            tbPCBASN.Background = System.Windows.Media.Brushes.LightGray;
            tbPCBASN.KeyDown += tbPCBASN_KeyDown;
            //tbSelectAction.MouseDown += tbSelectAction_MouseDown;
            bPCBASN.Child = tbPCBASN;


            //////////////////////////////////////////////
            //Label lbSelectAction = new Label();
            lbSIGKNOWSN.Content = ShopFloorLabel.SIGKNOWSN;

            //Border bSelectAction = new Border();
            bSIGKNOWSN.Width = 200;
            bSIGKNOWSN.Height = 30;
            bSIGKNOWSN.BorderBrush = System.Windows.Media.Brushes.Black;
            bSIGKNOWSN.BorderThickness = new Thickness(4);

            //TextBox tbSelectAction = new TextBox();
            tbSIGKNOWSN.Height = 30;
            tbSIGKNOWSN.Width = 200;
            tbSIGKNOWSN.Text = Global.gEMPTY;
            tbSIGKNOWSN.Background = System.Windows.Media.Brushes.LightGray;
            tbSIGKNOWSN.KeyDown += tbSIGKNOWSN_KeyDown;
            //tbSelectAction.MouseDown += tbSelectAction_MouseDown;
            bSIGKNOWSN.Child = tbSIGKNOWSN;


            bRESULT.Width = 350;
            bRESULT.Height = 30;
            lbRESULT.Background = System.Windows.Media.Brushes.White;
            //bRESULT.BorderBrush = System.Windows.Media.Brushes.Black;
            //bRESULT.BorderThickness = new Thickness(4);

            lbRESULT.Content = "";
            bRESULT.Child = lbRESULT;

            //StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Center;
            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(spUser);
            spMainBody.Children.Add(lbPCBASN);
            spMainBody.Children.Add(bPCBASN);
            spMainBody.Children.Add(lbSIGKNOWSN);
            spMainBody.Children.Add(bSIGKNOWSN);
            spMainBody.Children.Add(bRESULT);
            Utils.changeTextboxLang2Eng(tbPCBASN);

            Content = spMainBody;
            tbPCBASN.Focus();
            tbPCBASN.SelectAll();
            
        }

        private void tbPCBASN_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            lbRESULT.Content = "";
            lbRESULT.Background = System.Windows.Media.Brushes.White;
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
                    SNAssociate.ValidatePCBASN(Global.gPCBASN);
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                }
                catch (InvalidSerialNumberException isne)
                {
                    tbPCBASN.Clear();
                    tbSIGKNOWSN.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("PCBA序號 '" + Global.gPCBASN + "' 格式不符合規定.");
                }
                catch (PreviousErrorException pe)
                {
                    tbPCBASN.Clear();
                    tbSIGKNOWSN.Clear();
                    Utils.ErrorBeep();
                    MessageBox.Show("前一站測試未通過.");
                }
                catch (Exception ex)
                {
                    tbPCBASN.Focus();
                    tbPCBASN.Clear();
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                }
            }
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
                    SNAssociate.ValidateSIGKNOWSN(Global.gSIGKNOWSN);
                    SNAssociate.PrecheckAssociation(Global.gPCBASN, Global.gSIGKNOWSN);
                    SNAssociate.dbassociate(Global.gPCBASN, Global.gSIGKNOWSN);
                    SNAssociate.dbchangehistory(Global.gPCBASN, Global.gOLDSIGKNOWSN, DBColPrefix.gStationE,"+ " + Global.gSIGKNOWSN);
                    lbRESULT.Background = System.Windows.Media.Brushes.Green;
                    lbRESULT.Content = "上蓋序號 '" + tbSIGKNOWSN.Text + "' 已成功搭配 PCBA '" + tbPCBASN.Text + "'.";
                    tbSIGKNOWSN.Clear();
                    tbSIGKNOWSN.Focus();
                    tbPCBASN.Clear();
                    tbPCBASN.Focus();

                }
                catch (ResultUnchangedException rx)
                {
                    lbRESULT.Background = System.Windows.Media.Brushes.Green;
                    lbRESULT.Content = "上蓋序號 '" + tbSIGKNOWSN.Text + "' 已成功搭配 PCBA '" + tbPCBASN.Text + "'.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                }
                tbSIGKNOWSN.Clear();
                tbSIGKNOWSN.Focus();
                tbPCBASN.Clear();
                tbPCBASN.Focus();
            }
        }
    }
}
