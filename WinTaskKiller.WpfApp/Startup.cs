using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using MiniMvvm.Framework;
using MiniMvvm.Framework.Contracts;
using WinTaskKiller.Logic.Contract;
using WinTaskKiller.Logic.Service;
using WinTaskKiller.WpfApp.Contracts;
using WinTaskKiller.WpfApp.Models;
using WinTaskKiller.WpfApp.ViewModels;

namespace WinTaskKiller.WpfApp
{
    public class Startup : IStartup
    {
        // Define the parts of your main UI
        public static string Titlebar = "Titlebar";
        public static string Content = "Content";

        private IContainer _container;

        /// <summary>
        /// Resolves the given types from the dependency locator.
        /// </summary>
        /// <typeparam name="T">Type identifier.</typeparam>
        /// <returns>Instance of the given type.</returns>
        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        /// <summary>
        /// Resolves the given types from the dependency locator.
        /// </summary>
        /// <typeparam name="T">Type identifier.</typeparam>
        /// <param name="type">Type identifier</param>
        /// <returns>Instance of the given type.</returns>
        public object Resolve(Type type)
        {
            if (_container == null)
            {
                throw new ArgumentException("Container not initialized! Call LoadDependencies first!");
            }

            return _container.Resolve(type);
        }

        /// <summary>
        /// Loads all dependencies into the locator.
        /// </summary>
        public void LoadDependencies(Window window)
        {
            // Define default classes
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IStartup>();
            builder.RegisterInstance(new Navigator(window, this)).As<INavigator>();
            builder.RegisterInstance(window);

            // Define personal classes
            builder.RegisterType<TitleBarViewModel>().As<ITitleBarViewModel>();
            builder.RegisterType<WinTasksViewModel>().As<IWinTasksViewModel>();
            builder.RegisterType<WinTasksModel>().As<IWinTasksModel>();
            builder.RegisterType<WinTaskService>().As<IWinTaskService>();
            builder.RegisterType<IconService>().As<IIconService>();
            builder.RegisterType<KeyboardHookService>().As<IKeyboardHookService>();

            _container = builder.Build();
        }

        /// <summary>
        /// Gets called when the program window launches to display the initial view.s
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnStart(INavigator navigator)
        {
            // Show the default pages
            await navigator.Navigate<ITitleBarViewModel>(Titlebar);
            await navigator.Navigate<IWinTasksViewModel>(Content);
        }
    }
}