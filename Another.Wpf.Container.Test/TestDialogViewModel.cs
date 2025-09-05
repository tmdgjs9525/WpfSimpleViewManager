using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfSimpleViewManager.Dialog;
using WpfSimpleViewManager.Parameter;

namespace WpfSimpleViewManager.Test
{
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
}
