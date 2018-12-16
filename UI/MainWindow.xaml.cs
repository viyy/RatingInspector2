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

namespace RatingInspector2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
        }

        private void CloseAllOverlays(object sender, RoutedEventArgs e)
        {
            ExportOverlay.Visibility = Visibility.Collapsed;
            SettingsOverlay.Visibility = Visibility.Collapsed;
            ImportOverlay.Visibility = Visibility.Collapsed;
            UpdateOverlay.Visibility = Visibility.Collapsed;
            SearchOverlay.Visibility = Visibility.Collapsed;
        }

        private void ShowExportOverlay(object sender, RoutedEventArgs e)
        {
            CloseAllOverlays(sender, e);
            ExportOverlay.Visibility = Visibility.Visible;
        }
        private void ShowImportOverlay(object sender, RoutedEventArgs e)
        {
            CloseAllOverlays(sender, e);
            ImportOverlay.Visibility = Visibility.Visible;
        }
        private void ShowSettingsOverlay(object sender, RoutedEventArgs e)
        {
            CloseAllOverlays(sender, e);
            SettingsOverlay.Visibility = Visibility.Visible;
        }
        private void ShowUpdateOverlay(object sender, RoutedEventArgs e)
        {
            CloseAllOverlays(sender, e);
            UpdateOverlay.Visibility = Visibility.Visible;
        }
        private void ShowSearchOverlay(object sender, RoutedEventArgs e)
        {
            CloseAllOverlays(sender, e);
            SearchOverlay.Visibility = Visibility.Visible;
        }
    }
}
