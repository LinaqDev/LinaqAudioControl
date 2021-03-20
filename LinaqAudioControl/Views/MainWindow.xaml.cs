using LinaqAudioControl.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LinaqAudioControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        } 

        private void Slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is Slider s)
            {
                // Kill logical focus
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(s), null);
                // Kill keyboard focus
                Keyboard.ClearFocus();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var dc = DataContext as MainViewModel;
            dc.SaveSettings();

            base.OnClosing(e);
        }
    }
}
