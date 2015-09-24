using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SigknowShopFloor
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for StationE.xaml
    /// </summary>
    public partial class StationE : Window
    {
        Label lbStation = new Label();
        Label lbUser = new Label();
        Label lbUsertitle = new Label();
        StackPanel spUser = new StackPanel();
        Label lbSelectAction = new Label();
        Border bSelectAction = new Border();
        //Border bRESULT = new Border();
        TextBox tbSelectAction = new TextBox();
        //Label lbRESULT = new Label();
        //TextBox tbRESULT = new TextBox();
        StackPanel spMainBody = new StackPanel();

        private MainWindow mainwindow;

        public StationE()
        {
            InitializeComponent();
            ConstructObjects();
            Closing += OnWindowClosing;
        }
        public StationE(MainWindow mw): this()
        {
            mainwindow = mw;
        }
        public void ConstructObjects()
        {
            //Label lbStation = new Label();
            lbStation.Content = StationName.gStationE;
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
            lbSelectAction.Content = ShopFloorLabel.SelectAction;

            //Border bSelectAction = new Border();
            bSelectAction.Width = 200;
            bSelectAction.Height = 30;
            bSelectAction.BorderBrush = System.Windows.Media.Brushes.Black;
            bSelectAction.BorderThickness = new Thickness(4);

            //TextBox tbSelectAction = new TextBox();
            tbSelectAction.Height = 30;
            tbSelectAction.Width = 200;
            tbSelectAction.Text = Global.gEMPTY;
            tbSelectAction.Background = System.Windows.Media.Brushes.LightGray;
            tbSelectAction.KeyDown += tbSelectAction_KeyDown;
            //tbSelectAction.MouseDown += tbSelectAction_MouseDown;
            bSelectAction.Child = tbSelectAction;

            //StackPanel spMainBody = new StackPanel();
            spMainBody.HorizontalAlignment = HorizontalAlignment.Center;
            spMainBody.Children.Add(lbStation);
            spMainBody.Children.Add(spUser);
            spMainBody.Children.Add(lbSelectAction);
            spMainBody.Children.Add(bSelectAction);
            Utils.changeTextboxLang2Eng(tbSelectAction);

            Content = spMainBody;
            tbSelectAction.Focus();
            tbSelectAction.SelectAll();

        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            mainwindow.Visibility = Visibility.Visible;
        }

        private void tbSelectAction_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Background = System.Windows.Media.Brushes.White;
            if (e.Key.ToString() == "Return")
            {
                if (textBox.Text != "")
                {
                    Global.gSNAction = textBox.Text;
                }
                else
                    return;
                try
                {
                    tbSelectAction.Clear();
                    SNAssociate.ValidateActionCode(Global.gSNAction);
                    switch (Global.gSNAction)
                    {
                        case Barcode.ASSOCIATE:
                            var wAssociate = new StationEassociate(this);
                            this.Hide();
                            wAssociate.ShowDialog();
                            break;
                        case Barcode.DISASSOCIATE:
                            var wDisassociate = new StationEdisassociate(this);
                            this.Hide();
                            wDisassociate.ShowDialog();
                            break;
                        case Barcode.QUERY:
                        default:
                            break;

                    }
                }
                catch (Exception ex)
                {
                    tbSelectAction.Focus();
                    tbSelectAction.Clear();
                    MessageBox.Show(ex.ToString());
                    Utils.ErrorBeep();
                    tbSelectAction.Focus();
                }

            }

        }
    }
}
