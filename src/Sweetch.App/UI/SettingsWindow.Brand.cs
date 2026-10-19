using System.Windows;
using System.Windows.Controls;
using static Sweetch.App.UI.Controls;
using static Sweetch.Core.Loc;
namespace Sweetch.App.UI;
internal sealed partial class SettingsWindow
{
    private void AboutPage(Panel root)
    {
        root.Children.Add(Text("Sweetch Desktop 1.0.0", 28, FontWeights.SemiBold));
        root.Children.Add(Card(T("Desktop edition", "Настольная версия"), "yasinsckijtema1907-eng / Sweetch-Desktop",
            Button("GitHub", () => Shell.Updater.Open(Brand.Repository))));
        root.Children.Add(Card(T("Original project", "Исходный проект"), "Sweetch — @mmmorozov; original code — Metamorfolog. MIT License.",
            Button(T("Source and credits", "Исходник и авторы"), () => Shell.Updater.Open(Brand.Upstream))));
        root.Children.Add(Card(T("Feedback", "Обратная связь"), T("Suggestions and bug reports on GitHub.", "Предложения и сообщения об ошибках на GitHub."),
            Button("GitHub Issues", () => Shell.Updater.Open(Brand.Channel))));
        root.Children.Add(Card(T("Updates", "Обновления"), T("Download new versions from this project's Releases page.", "Новые версии доступны на странице Releases этого проекта."),
            Button(T("Open releases", "Открыть релизы"), () => Shell.Updater.Open(Brand.Releases))));
        var footer = Text(T("Local processing. Your files stay on your computer.", "Локальная обработка. Ваши файлы остаются на компьютере."), 12, brush: Theme.Muted);
        footer.Margin = new Thickness(2, 24, 0, 0); root.Children.Add(footer);
    }
}
