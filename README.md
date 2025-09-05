# WpfSimpleViewManager

WpfSimpleViewManager는 WPF MVVM 애플리케이션을 위한 간단하고 가벼운 뷰(View) 관리 라이브러리입니다. **Region 기반**의 화면 탐색과 직관적인 다이얼로그 서비스를 `Microsoft.Extensions.DependencyInjection`을 통해 손쉽게 구성하고 사용할 수 있도록 도와줍니다.

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

### ViewManager 등록
```

public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
{
    return hostBuilder.ConfigureServices(services =>
    {
        services.AddWpfSimpleViewManager();
    });
}
```

