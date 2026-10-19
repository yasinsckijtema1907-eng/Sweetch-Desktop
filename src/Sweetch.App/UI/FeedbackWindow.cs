using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Sweetch.App.UI.Controls;
using static Sweetch.Core.Loc;
namespace Sweetch.App.UI;
internal sealed class FeedbackWindow : Window
{
    public const string Website = Brand.Channel;
    internal FeedbackWindow()
    {
        Theme.Refresh();
        Title = T("Sweetch — feedback", "Sweetch — обратная связь");
        Width = 540; SizeToContent = SizeToContent.Height;
        ResizeMode = ResizeMode.NoResize; WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/Sweetch.ico"));
        Background = Theme.Page; Foreground = Theme.Text; FontFamily = Theme.UiFont;
        var root = new StackPanel { Margin = new Thickness(28) };
        root.Children.Add(Text(T("Let's make Sweetch better", "Сделаем Sweetch лучше"), 25, FontWeights.SemiBold));
        var intro = Text(T("Ideas, questions and feedback — on GitHub.", "Идеи, вопросы и обратная связь — на GitHub."), 14, brush: Theme.Muted);
        intro.Margin = new Thickness(0, 12, 0, 22); root.Children.Add(intro);
        root.Children.Add(Button("GitHub Issues", () => Shell.Updater.Open(Brand.Channel), primary: true));
        Content = root;
    }
    public static void ShowSingle() => Shell.Updater.Open(Brand.Channel);
}
