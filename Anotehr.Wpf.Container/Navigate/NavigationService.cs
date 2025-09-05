using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfSimpleViewManager.Parameter;

namespace WpfSimpleViewManager.Navigate
{

    internal class NavigationService : INavigationService, INavigationRegister, IRegionRegister
    {
        // object는 ViewModel의 Type이 될 수도 있고, ViewModel의 인스턴스가 될 수도 있음
        private readonly Dictionary<string, Tuple<Type, object>> _viewDictionary = new();

        //어태치 프로퍼티로 ContentControl 사용하는 곳에서 등록된다.
        private readonly Dictionary<string, ContentControl> _regionDictionary = new();

        //di 등록용
        //private readonly IServiceCollection _serviceCollection;

        internal NavigationService()
        {

        }

        public void RegisterRegion(string regionName, ContentControl control)
        {
            _regionDictionary[regionName] = control;
        }

        //CommandParameter는 문자열로 들어오니 viewName을 string으로
        public void NavigateTo(string regionName, string viewName, Parameters? parameters = null)
        {
            var viewInfo = _viewDictionary[viewName];
            var viewType = viewInfo.Item1;
            var viewModelInfo = viewInfo.Item2; // Type 또는 인스턴스

            // Region 등록되어 있는지 확인
            if (_regionDictionary.ContainsKey(regionName) == false)
            {
                throw new ArgumentNullException($"Can't find '{regionName}' region");
            }

            // View 등록되어 있는지 확인
            if (_viewDictionary.ContainsKey(viewName) == false)
            {
                throw new ArgumentNullException($"Can't find '{viewName}' from _viewDictionary ");
            }

           
            var control = Ioc.Default.GetRequiredService(viewType) as UserControl;

            // Control 등록 여부 확인
            if (control == null)
            {
                throw new ArgumentNullException($"Can't find '{viewName}' from Di Container");
            }

            // ViewModelInfo가 Type인지 인스턴스인지 확인하여 분기 처리
            object viewModelInstance;
            if (viewModelInfo is Type viewModelType)
            {
                //Type이면 DI 컨테이너에서 인스턴스를 가져옴
                viewModelInstance = Ioc.Default.GetRequiredService(viewModelType);
            }
            else
            {
                //인스턴스면 해당 인스턴스를 그대로 사용
                viewModelInstance = viewModelInfo;
            }

            control.DataContext = viewModelInstance;

            //ViewModel에 Navigate 됐다고 호출
            if (control.DataContext is INavigateAware navigateAware)
            {
                navigateAware.NavigateTo(parameters ?? new Parameters());
            }

            //Region Navigate
            _regionDictionary[regionName].Content = control;

            // TODO : 로깅
        }


        public void AddTransientNavigation<TView, TViewModel>() where TView : Control
                                                                 where TViewModel : IViewModelBase
        {
            _viewDictionary[typeof(TView).Name] =
                new Tuple<Type, object>(typeof(TView), typeof(TViewModel)); 
        }

        public void AddSingletonNavigation<TView, TViewModel>() where TView : Control
                                                                where TViewModel : IViewModelBase
        {
            _viewDictionary[typeof(TView).Name] =
                new Tuple<Type, object>(typeof(TView), typeof(TViewModel));
        }

        public void AddSingletonNavigation<TInterface, TImplementationView, TViewModel>() where TInterface : class               // TInterface는 참조 형식이어야 함
                                                                                         where TImplementationView : Control, TInterface // TImplementation은 Control을 상속하고 TInterface를 구현해야 함
                                                                                         where TViewModel : IViewModelBase
        {
            string viewName = typeof(TInterface).Name.Substring(1);

            _viewDictionary[viewName] = new Tuple<Type, object>(typeof(TImplementationView), typeof(TViewModel));
        }

        public void AddTransientNavigation<TInterface, TImplementationView, TViewModel>() where TInterface : class               // TInterface는 참조 형식이어야 함
                                                                                          where TImplementationView : Control, TInterface // TImplementation은 Control을 상속하고 TInterface를 구현해야 함
                                                                                          where TViewModel : IViewModelBase
        {
            string viewName = typeof(TInterface).Name.Substring(1);

            _viewDictionary[viewName] = new Tuple<Type, object>(typeof(TImplementationView), typeof(TViewModel));
        }

        /// <summary>
        /// View와 미리 생성된 ViewModel 인스턴스를 Transient 라이프타임으로 등록합니다.
        /// View는 탐색 시마다 새로 생성되지만, ViewModel은 제공된 인스턴스를 공유합니다.
        /// </summary>
        public void AddTransientNavigation<TView>(IViewModelBase viewModelInstance) where TView : Control
        {
            _viewDictionary[typeof(TView).Name] =
                new Tuple<Type, object>(typeof(TView), viewModelInstance);
        }

        /// <summary>
        /// View와 미리 생성된 ViewModel 인스턴스를 Singleton 라이프타임으로 등록합니다.
        /// View는 DI 컨테이너에 Singleton으로 등록되어 있어야 하며, ViewModel은 제공된 인스턴스를 공유합니다.
        /// </summary>
        public void AddSingletonNavigation<TView>(IViewModelBase viewModelInstance) where TView : Control
        {
            _viewDictionary[typeof(TView).Name] =
                new Tuple<Type, object>(typeof(TView), viewModelInstance);
        }
    }
}
