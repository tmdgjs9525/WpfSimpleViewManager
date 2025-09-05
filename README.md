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

### 🗂 Region 등록 (Navi)

XAML에서 `ContentControl` 같은 컨트롤에  
regionManager:RegionManager.RegionName="MainRegion" 속성을 지정하여 Region을 등록합니다.

```
<Window
    x:Class="YourApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:regionmanager="clr-namespace:WpfSimpleViewManager.Region;assembly=WpfSimpleViewManager">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- MainRegion 등록 -->
        <ContentControl Grid.Row="0"
                        regionmanager:RegionManager.RegionName="MainRegion" />

        <Button Grid.Row="1"
                Height="50"
                Command="{Binding NavigateCommand}"
                Content="Go" />
    </Grid>
</Window>
```

### 📌 Use

#### 호출하는 곳

- **Navigation**  
  'INavigationService'를 주입받아  
  'NavigateTo("RegionName", "ViewName", Parameters)' 메서드를 호출하면 지정한 Region에 View가 교체됩니다.

- **Dialogs**  
  'IDialogService'를 주입받아  
  'ShowDialog("DialogName", Parameters, callback)' 메서드를 호출하면 다이얼로그를 띄울 수 있습니다.
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
#### 🔄 INavigateAware & IDialogAware

**Parameter Handling**  
ViewModel에서 `INavigateAware` 와 `IDialogAware` 인터페이스를 구현하면,  
뷰가 열리거나 내비게이션될 때 파라미터를 안전하게 전달받고  
닫힐 때 관련 로직을 처리할 수 있습니다.
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
