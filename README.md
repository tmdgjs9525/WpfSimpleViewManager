# WpfSimpleViewManager

WpfSimpleViewManager는 WPF MVVM 애플리케이션을 위한 간단하고 가벼운 뷰(View) 관리 라이브러리입니다. 

**Region 기반**의 화면 탐색과 직관적인 다이얼로그 서비스를 `Microsoft.Extensions.DependencyInjection`을 통해 손쉽게 구성하고 사용할 수 있도록 도와줍니다.

Prism을 모방했습니다.

## ✨ 주요 기능

- **Region 기반 내비게이션**: UI의 특정 영역(`Region`)을 지정하여 해당 부분의 콘텐츠만 교체할 수 있습니다.
- **다이얼로그 관리**: `DialogService`를 통해 간단하게 Modal/Modaless 다이얼로그를 띄울 수 있습니다.
- **생명 주기 관리**: View와 ViewModel을 `Transient`(일회성) 또는 `Singleton`(단일 인스턴스)으로 등록할 수 있습니다.
- **유연한 View 등록**: View를 구체 클래스뿐만 아니라 인터페이스로도 등록하여 유연하게 교체할 수 있습니다.
- **MVVM 친화적**: `INavigateAware`, `IDialogAware` 인터페이스를 통해 ViewModel 간의 안전한 파라미터 전달을 지원합니다.

---

## 의존성

- **CommunityToolkit.Mvvm
- **Microsoft.Extensions.DependencyInjection


## 사용 방법

### Di 등록
```

public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
{
    return hostBuilder.ConfigureServices(services =>
    {
        services.AddWpfSimpleViewManager();

        //View는 UserControl로 만들어질것을 예상하고 있음
        //ViewModel은 WpfSimpleViewmanager의 IViewModelBase를 구현해야함
        //ViewModelBase를 만들어 IViewModelBase 구현하세요

        //Navigate
        services.AddSingletonNavigation<BView, BViewModel>();
        services.AddTransientNavigation<BView, BViewModel>();

        //Use Instance
        var vm = new CommonViewModel();
        services.AddTransientNavigation<AView>(vm);
        services.AddSingletonNavigation<BView>(vm);

        //Use Interface
        services.AddSingletonNavigation<IMainView, MainWindow, MainWindowViewModel>();
   
        //Dialog
        //Dialog의 ViewModel은 IDialogAware를 필수적으로 구현해야 함
        services.AddTransientDialog<TestDialog,TestDialogViewModel>();
        services.AddSingletonDialog<TestDialog,TestDialogViewModel>();

    });
}
```

### Region 등록 (Navi)
```
<Window
    --생략--
    xmlns:regionmanager="clr-namespace:WpfSimpleViewManager.Region;assembly=WpfSimpleViewManager"
>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
            <ContentControl Grid.Row="0" regionmanager:RegionManager.RegionName="MainRegion" />
            <Button Command="{Binding NavigateCommand}" Height="50" Grid.Row="1"/>
    </Grid>
</Window>

```

### Use
#### 호출하는 곳
```
private readonly INavigationService _navigationService;
private readonly IDialogService _dialogService;

public MainWindowVIewModel(INavigationService navigationService, IDialogService dialogService)
{
     _navigationService = navigationService;
    _dialogService = dialogService;
}

[RelayCommand]
private void Navigate()
{
    _navigationService.NavigateTo("MainRegion", "BView");

    //With Parameters
    _navigationService.NavigateTo(RegionNames.MainRegion, ViewNames.MainView, new Parameters()
    {
        {"numbers", new List<int>() {1,3,5,67} },
    });
}

[RelayCommand]
private void Dialog()
{
    // 다이얼로그의 Owner는 현재 활성화중인 Window
    _dialogService.ShowDialog("TestDialog");

    //With Parameters
    _dialogService.ShowDialog("TestDialog", new Parameters()
   {
     {"Content", "Hello Dialog"},
   }, callback =>
   {
     if(result.Success)
     {
       //Do Work
     }
     
     if(result.Parameters.ContainsKey("key"))
     {
       var key = result.Parameters.GetValue<int>("Key");
     }
   });
}
```
#### INaviateAware, IDialogAware
```
INavigateAware
internal partial class CommonViewModel : ViewModelBase, INavigateAware
{
    [ObservableProperty]
    private int _count = 0;

    public void NavigateTo(Parameters parameters)
    {
        Count++;
    }
}
```
```
internal partial class TestDialogViewModel : ViewModelBase, IDialogAware
{
    public string? Title { get; set; }

    public event Action<IDialogResult?>? RequestClose;

    public bool CanCloseDialog()
    {
        return true;   
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(Parameters parameters)
    {
    }

    [RelayCommand]
    private void Exit()
    {
         RequestClose?.Invoke(new DialogResult { Success = true, Parameters = new Parameters() });
    }
}

```
