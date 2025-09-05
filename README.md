# WpfSimpleViewManager

**WPF MVVM**용 뷰(View) 관리 라이브러리입니다.  
`Microsoft.Extensions.DependencyInjection` 기반으로 **Region 내비게이션**과 **다이얼로그 서비스**를 간단하게 구성할 수 있습니다.  

Prism을 모방했습니다.

## ✨ 주요 기능

- **Region 기반 내비게이션**: UI의 특정 영역(`Region`)을 지정하여 해당 부분의 콘텐츠만 교체할 수 있습니다.
- **다이얼로그 관리**: `DialogService`를 통해 간단하게 Modal/Modaless 다이얼로그를 띄울 수 있습니다.
- **생명 주기 관리**: View와 ViewModel을 `Transient`(일회성) 또는 `Singleton`(단일 인스턴스)으로 등록할 수 있습니다.
- **유연한 View 등록**: View를 구체 클래스뿐만 아니라 인터페이스로도 등록하여 유연하게 교체할 수 있습니다.
- **MVVM 친화적**: `INavigateAware`, `IDialogAware` 인터페이스를 통해 ViewModel 간의 안전한 파라미터 전달을 지원합니다.

---

## 의존성

- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/introduction)  
- [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)  


## 사용 방법

### Di 등록

services.AddWpfSimpleViewManager()를 통해 서비스를 등록하고, AddSingletonNavigation, AddTransientNavigation, AddSingletonDialog, AddTransientDialog 등의 확장 메서드를 사용하여 View와 ViewModel을 등록합니다.
> ⚠️ 규약  
> - **ViewModel**은 `WpfSimpleViewManager.IViewModelBase`를 구현해야 합니다.  
> - **View**는 `System.Windows.Controls.Control` 파생이어야 합니다.
```

public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
{
    return hostBuilder.ConfigureServices(services =>
    {
        services.AddWpfSimpleViewManager();

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
Region 등록: XAML에서 ContentControl 같은 컨트롤에 regionManager:RegionManager.RegionName="MainRegion"과 같이 Region 이름을 지정합니다.
```
<Window
    --skip--
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
Navigation: Inject INavigationService and call the MapsTo("RegionName", "ViewName", Parameters) method to switch views.

Dialogs: Inject IDialogService and call the ShowDialog("DialogName", Parameters, callback) method to open a dialog.
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
Parameter Handling: Implement the INavigateAware and IDialogAware interfaces in your ViewModel to receive parameters when a view is opened or navigated to, and to handle logic when it closes.
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
