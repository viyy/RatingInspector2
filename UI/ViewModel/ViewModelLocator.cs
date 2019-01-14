/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:UI"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using Interfaces;
using Services;
using DAL;

namespace UI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IInfo, InfoService>();
            SimpleIoc.Default.Register<IUpdateService, UpdateService>();
            SimpleIoc.Default.Register<IImportService, ImportService>();
            SimpleIoc.Default.Register<IExportService, ExportService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<UpdateViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ImportViewModel>();
            SimpleIoc.Default.Register<ExportViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public UpdateViewModel UpdateVM => ServiceLocator.Current.GetInstance<UpdateViewModel>();

        public SettingsViewModel SettingsVM => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public ImportViewModel ImportVM => ServiceLocator.Current.GetInstance<ImportViewModel>();

        public ExportViewModel ExportVM => ServiceLocator.Current.GetInstance<ExportViewModel>();

        public static void Cleanup()
        {
        }
    }
}