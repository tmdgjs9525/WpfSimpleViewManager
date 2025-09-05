using WpfSimpleViewManager.Core;
using WpfSimpleViewManager.Navigate;
using WpfSimpleViewManager.Parameter;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSimpleViewManager.Test.TestViewModels
{
    internal partial class CommonViewModel : ObservableObject ,IViewModelBase, INavigateAware
    {
        [ObservableProperty]
        private int _count = 0;

        public void NavigateTo(Parameters parameters)
        {
            Count++;
        }
    }
}
