using WpfSimpleViewManager.Dialog;
using WpfSimpleViewManager.Extensions;
using WpfSimpleViewManager.Navigate;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSimpleViewManager.Extensions
{
    public static class ServiceAdder
    {
        public static IServiceCollection AddWpfContainerService(this IServiceCollection services)
        {
            ///////////////////////////naviagtion
            NavigationService navigationService = new NavigationService();

            // NavigationService를 싱글톤으로 등록
            services.AddSingleton<INavigationService>(navigationService);

            // 인터페이스들도 같은 인스턴스로 등록
            services.AddSingleton<INavigationService>(navigationService);
            services.AddSingleton<INavigationRegister>(navigationService);
            services.AddSingleton<IRegionRegister>(navigationService);

            NavigationServiceExtensions._navigationRegister = navigationService;




            /////////////////////////Dialog
            DialogService dialogService = new DialogService();
            // DialogService를 싱글톤으로 등록
            services.AddSingleton<IDialogService>(dialogService);

            // 인터페이스들도 같은 인스턴스로 등록
            services.AddSingleton<IDialogService>(dialogService);
            services.AddSingleton<IDialogRegister>(dialogService);

            DialogServiceExtensions._dialogRegister = dialogService;


            return services;
        }
    }
}
