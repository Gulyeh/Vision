using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using VisionClient.Extensions;
using VisionClient.Views;

namespace VisionClient
{
    internal class LoadingBootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<LoadingWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterDependenciesExtension.RegisterLoadingDependencies(containerRegistry);
        }
    }
}
