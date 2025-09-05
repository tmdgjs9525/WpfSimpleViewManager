using System.Windows.Controls;
using WpfSimpleViewManager.Parameter;

namespace WpfSimpleViewManager.Dialog
{
    public interface IDialogService
    {
        public void ShowDialog(string viewName, Parameters? parameters = null, Action<IDialogResult>? callback = null, StartPosition? startPosition = null);

        public void Show(string viewName, Parameters? parameters = null,  StartPosition? startPosition = null);
    }

    public interface IDialogRegister
    {
        public void AddTransientDialog<TView, TViewModel>() where TView : Control
                                                            where TViewModel : IViewModelBase, IDialogAware;
        public void AddSingletonDialog<TView, TViewModel>() where TView : Control
                                                            where TViewModel : IViewModelBase, IDialogAware;
    }

    public interface IDialogResult
    {
        public bool Success { get; set; }
        public Parameters Parameters { get; set; }
    }

    public interface IDialogAware
    {
        bool CanCloseDialog();
        void OnDialogClosed();
        void OnDialogOpened(Parameters parameters);
        string Title { get; set; }

        event Action<IDialogResult?>? RequestClose;
    }

    public enum StartPosition
    {
        Center,
        Left,
        Top,
        Right,
        Bottom,
        LeftUp,
        RightUp,
        LeftDown,
        RightDown,
    }
}
