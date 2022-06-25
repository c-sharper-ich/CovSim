using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CovidSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int StartInfect = 1;
        const int Person = 40;
        const bool PersonMasked = false;

        Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void BTC(object sender, RoutedEventArgs e)
        {
            try
            {

                new Window1(Convert.ToInt32(tb_n0.Text), Convert.ToInt32(tb_n.Text), cb_allow.IsChecked.Value, Convert.ToInt32(tb_q.Text), Convert.ToInt32(tb_t.Text), Convert.ToInt32(tb_p.Text), 60).Show();
            }
            catch
            {
                MessageBox.Show("テキストボックスの内容に問題があると考えられます。再度確認してください。");
            }

        }
    }
}

