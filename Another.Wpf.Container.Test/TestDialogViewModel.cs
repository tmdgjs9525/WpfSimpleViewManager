using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfSimpleViewManager.Core;
using WpfSimpleViewManager.Dialog;
using WpfSimpleViewManager.Parameter;

namespace WpfSimpleViewManager.Test
{
    public partial class TestDialogViewModel : ObservableObject,IViewModelBase, IDialogAware
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
        private void Loaded()
        {

        }

        [RelayCommand]
        private void Exit()
        {
             RequestClose?.Invoke(new DialogResult { Success = true, Parameters = new Parameters() });
        }
    }
}
