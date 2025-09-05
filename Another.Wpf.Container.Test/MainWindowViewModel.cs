using WpfSimpleViewManager.Dialog;
using WpfSimpleViewManager.Navigate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSimpleViewManager.Test
{
    public partial class MainWindowViewModel : ObservableObject
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
            //_dialogService.ShowDialog("TestDialog", startPosition: StartPosition.Left);

        }
    }
}
