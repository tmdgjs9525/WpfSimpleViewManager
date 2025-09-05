using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfSimpleViewManager.Extensions;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace WpfSimpleViewManager.Dialog
{
    /// <summary>
    /// DialogBase.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DialogBase : Window
    {
        private bool _positioned = false;
        private readonly StartPosition? _startPosition;
        public DialogBase(StartPosition? startPosition = null)
        {
            InitializeComponent();

            Owner = WindowUtil.GetActiveWindow();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            _startPosition = startPosition;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Control content = dialogContent.Content as Control ?? throw new ArgumentNullException("ContentControls Content is not UserControl. should be UserControl");
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!_positioned && _startPosition is not null)
            {
                if (_startPosition == StartPosition.Center)
                    CenterToOwner();           // 주모니터 고정 금지, 오너 기준
                else
                    PositionFromCursor(_startPosition.Value);

                _positioned = true;
            }

            Opacity = 1;
        }

        private void CenterToOwner()
        {
            var owner = Owner ?? WindowUtil.GetActiveWindow();
            if (owner == null) return;

            Left = owner.Left + (owner.ActualWidth - Width) / 2;
            Top = owner.Top + (owner.ActualHeight - Height) / 2;
        }

        private void PositionFromCursor(StartPosition pos)
        {
            var (cursor, workArea) = MonitorUtil.GetCursorAndWorkAreaWpf(this);

            // 기본값: 가운데 스케일 anchor
            dialogContent.RenderTransformOrigin = new Point(0.5, 0.5);

            switch (pos)
            {
                case StartPosition.Left:
                    dialogContent.RenderTransformOrigin = new Point(0, 0.5);
                    Left = cursor.X;
                    Top = cursor.Y - Height / 2;
                    break;

                case StartPosition.Right:
                    dialogContent.RenderTransformOrigin = new Point(1, 0.5);
                    Left = cursor.X - Width;
                    Top = cursor.Y - Height / 2;
                    break;

                case StartPosition.Top:
                    dialogContent.RenderTransformOrigin = new Point(0.5, 0);
                    Left = cursor.X - Width / 2;
                    Top = cursor.Y;
                    break;

                case StartPosition.Bottom:
                    dialogContent.RenderTransformOrigin = new Point(0.5, 1);
                    Left = cursor.X - Width / 2;
                    Top = cursor.Y - Height;
                    break;

                case StartPosition.LeftUp:
                    dialogContent.RenderTransformOrigin = new Point(0, 0);
                    Left = cursor.X;
                    Top = cursor.Y;
                    break;

                case StartPosition.RightUp:
                    dialogContent.RenderTransformOrigin = new Point(1, 0);
                    Left = cursor.X - Width;
                    Top = cursor.Y;
                    break;

                case StartPosition.LeftDown:
                    dialogContent.RenderTransformOrigin = new Point(0, 1);
                    Left = cursor.X;
                    Top = cursor.Y - Height;
                    break;

                case StartPosition.RightDown:
                    dialogContent.RenderTransformOrigin = new Point(1, 1);
                    Left = cursor.X - Width;
                    Top = cursor.Y - Height;
                    break;

                default:
                    CenterToOwner();
                    break;
            }

            // 해당 모니터 워킹영역 안으로 클램프
            Left = Math.Min(Math.Max(Left, workArea.Left), workArea.Right - Width);
            Top = Math.Min(Math.Max(Top, workArea.Top), workArea.Bottom - Height);
        }

        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxiMizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void AnimateWindowSize(double targetWidth, double targetHeight)
        {
            var scaleXAnim = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };

            var scaleYAnim = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = dialogContent.RenderTransform as ScaleTransform;
            transform?.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            transform?.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }


    }
}
