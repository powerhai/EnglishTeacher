using Microsoft.Practices.Unity;
using Prism.Unity;
using EngManageDesktop.Views;
using System.Windows;
using Prism.Modularity;
namespace EngManageDesktop
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return  new DirectoryModuleCatalog() { ModulePath = "Modules"};
        }
 
        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
