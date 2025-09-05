using CommunityToolkit.Mvvm.ComponentModel;
using WpfSimpleViewManager.Navigate;
using WpfSimpleViewManager.Parameter;

namespace WpfSimpleViewManager.Test.TestViewModels
{
    internal partial class CommonViewModel : ViewModelBase, INavigateAware
    {
        [ObservableProperty]
        private int _count = 0;

        public void NavigateTo(Parameters parameters)
        {
            Count++;
        }
    }
}
