using CommunityToolkit.Mvvm.Input;
using WpfSimpleViewManager.Dialog;
using WpfSimpleViewManager.Navigate;

namespace WpfSimpleViewManager.Test
{
    internal partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        public MainWindowViewModel(IDialogService dialogService, INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        bool _view = true;
        [RelayCommand]
        private void Click()
        {
            if(_view)
            {
                _navigationService.NavigateTo("MainRegion", "BView");
            }
            else
            {
                _navigationService.NavigateTo("MainRegion", "AView");
            }

            _view = !_view;
            _dialogService.ShowDialog("TestDialog", parameters : new Parameter.Parameters() 
            {

            }, callback: result =>
            {

            }
             );

        }
    }
}
