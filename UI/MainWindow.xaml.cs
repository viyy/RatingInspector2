using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Nelfias.Controls;
using UI.Utils;

namespace UI
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, Overlay> _overlays = new Dictionary<string, Overlay>();

        private void CloseAllOverlays(object sender, RoutedEventArgs e)
        {
            if (StateMachine.CurrentState != ProcessStates.Idle) return;
            foreach (var overlay in _overlays) overlay.Value.Visibility = Visibility.Collapsed;
        }

        private void ShowOverlay(object sender, RoutedEventArgs e)
        {
            if (StateMachine.CurrentState != ProcessStates.Idle) return;
            var tag = ((Button) sender).Tag.ToString();
            if (!_overlays.ContainsKey(tag)) return;
            CloseAllOverlays(sender, e);
            _overlays[tag].Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _overlays.Add("export", ExportOverlay);
            _overlays.Add("settings", SettingsOverlay);
            _overlays.Add("import", ImportOverlay);
            _overlays.Add("update", UpdateOverlay);
            _overlays.Add("search", SearchOverlay);
            //_overlays.Add("groups", GroupOverlay);
        }
    }
}