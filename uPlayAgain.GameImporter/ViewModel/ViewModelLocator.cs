/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:uPlayAgain.GameImporter"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using uPlayAgain.GameImporter.Service;

namespace uPlayAgain.GameImporter.ViewModel
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

            SimpleIoc.Default.Register<IConfigurationApplication, ConfigurationApplication>();
            SimpleIoc.Default.Register<IConnectionWebApi, ConnectionWebApi>();
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();


            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ImportGameViewModel>();
            SimpleIoc.Default.Register<ListGameViewModel>();
        }
        
        public MainViewModel Main
        {
            get
            {   
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ImportGameViewModel ImportGameView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImportGameViewModel>();
            }
        }

        public ListGameViewModel ListGameView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListGameViewModel>();
            }
        }

        public static void Cleanup()
        { }
    }
}