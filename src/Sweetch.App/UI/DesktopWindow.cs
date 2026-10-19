using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Sweetch.App.Shell;
using static Sweetch.App.UI.Controls;
using static Sweetch.Core.Loc;

namespace Sweetch.App.UI;

/// <summary>A desktop entry point for the conversion wheel.</summary>
internal sealed class DesktopWindow : Window
{
    private static DesktopWindow? _open;
    private readonly Action<IEnumerable<string>> _convert;
    private readonly Action _settings;
    private readonly List<string> _files = [];
    private ListBox _list = null!;
    private Button _start = null!;
    private TextBlock _count = null!;

    internal DesktopWindow(Action<IEnumerable<string>> convert, Action settings)
    {
        _convert = convert;
        _settings = settings;
        Width = 820; Height = 680; MinWidth = 660; MinHeight = 640;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/Sweetch.ico"));
        AllowDrop = true;
        DragOver += (_, e) =>
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        };
        Drop += (_, e) =>
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) AddFiles(paths);
            e.Handled = true;
        };
        Closed += (_, _) => _open = null;
        Rebuild();
    }

    public static void ShowSingle(Action<IEnumerable<string>> convert, Action settings)
    {
        _open ??= new DesktopWindow(convert, settings);
        _open.Rebuild();
        _open.Show();
        if (_open.WindowState == WindowState.Minimized) _open.WindowState = WindowState.Normal;
        _open.Activate();
    }

    private void Rebuild()
    {
        Title = "Sweetch Desktop";
        Background = Theme.Page; Foreground = Theme.Text;
        var root = new Grid { Margin = new Thickness(30) };
        foreach (var height in new[] { GridLength.Auto, GridLength.Auto, new GridLength(1, GridUnitType.Star), GridLength.Auto, GridLength.Auto })
            root.RowDefinitions.Add(new RowDefinition { Height = height });
        var header = new DockPanel { Margin = new Thickness(0, 0, 0, 24) };
        var settings = Button(T("Settings", "Настройки"), _settings, glyph: "\uE713");
        DockPanel.SetDock(settings, Dock.Right); header.Children.Add(settings);
        var titles = new StackPanel();
        titles.Children.Add(Text("Sweetch Desktop", 30, FontWeights.SemiBold));
        titles.Children.Add(Text(T("A new shape for your files.", "Новая форма ваших файлов."), 14, brush: Theme.Muted));
        header.Children.Add(titles); root.Children.Add(header);
        var intro = new StackPanel { Margin = new Thickness(0, 0, 0, 18) };
        intro.Children.Add(Text(T("Drop files here or choose them below", "Перетащите файлы сюда или выберите их кнопкой"), 19, FontWeights.SemiBold));
        intro.Children.Add(Text(T("Images · PDF · Documents · Spreadsheets · Archives · Audio and video", "Изображения · PDF · Документы · Таблицы · Архивы · Аудио и видео"), 13, brush: Theme.Muted));
        Grid.SetRow(intro, 1); root.Children.Add(intro);
        _list = new ListBox { Background = Theme.Panel, Foreground = Theme.Text, BorderBrush = Theme.PanelBorder,
            BorderThickness = new Thickness(1), Padding = new Thickness(12), MinHeight = 180, SelectionMode = SelectionMode.Extended };
        System.Windows.Automation.AutomationProperties.SetName(_list, T("Selected files", "Выбранные файлы"));
        Grid.SetRow(_list, 2); root.Children.Add(_list);
        var actions = new StackPanel { Margin = new Thickness(0, 16, 0, 0) };
        _count = Text("", 12, brush: Theme.Muted); actions.Children.Add(_count);
        var buttons = new WrapPanel { Margin = new Thickness(0, 10, 0, 0) };
        var choose = Button(T("Choose files", "Выбрать файлы"), ChooseFiles, glyph: "\uE8E5");
        var clear = Button(T("Clear list", "Очистить список"), () => { _files.Clear(); RefreshFiles(); });
        _start = Button(T("Choose format →", "Выбрать формат →"), () => _convert(_files.ToArray()), primary: true);
        foreach (var button in new[] { choose, clear, _start })
        {
            button.Margin = new Thickness(0, 0, 10, 8); buttons.Children.Add(button);
        }
        actions.Children.Add(buttons); Grid.SetRow(actions, 3); root.Children.Add(actions);
        var footer = new StackPanel { Margin = new Thickness(0, 14, 0, 0) };
        footer.Children.Add(Text(T("Choose files → choose a format in the wheel → get the result.", "Выберите файлы → выберите формат в круговом меню → получите результат."), 12, brush: Theme.Muted));
        footer.Children.Add(Text(T("Local processing. Originals stay intact. Closing this window keeps the tray icon running.", "Обработка на компьютере. Оригиналы сохраняются. После закрытия окна программа остаётся в трее."), 12, brush: Theme.Muted));
        var links = new WrapPanel { Margin = new Thickness(0, 10, 0, 0) };
        links.Children.Add(Button("GitHub", () => Updater.Open(Brand.Repository)));
        var releases = Button(T("Download releases", "Скачать обновления"), () => Updater.Open(Brand.Releases));
        releases.Margin = new Thickness(10, 0, 0, 0); links.Children.Add(releases);
        footer.Children.Add(links); Grid.SetRow(footer, 4); root.Children.Add(footer);
        Content = root; RefreshFiles();
    }

    private void ChooseFiles()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog { Multiselect = true, Title = T("Choose files to convert", "Выберите файлы для конвертации"), CheckFileExists = true };
        if (dialog.ShowDialog(this) == true) AddFiles(dialog.FileNames);
    }

    internal void AddFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths.Where(p => File.Exists(p) || Directory.Exists(p)))
            if (!_files.Contains(path, StringComparer.OrdinalIgnoreCase)) _files.Add(path);
        RefreshFiles();
    }

    private void RefreshFiles()
    {
        _list.Items.Clear();
        foreach (var path in _files)
        {
            var item = new StackPanel { Margin = new Thickness(2, 6, 2, 6), ToolTip = path };
            item.Children.Add(Text(Path.GetFileName(path), 14, FontWeights.SemiBold));
            item.Children.Add(Text(path, 11, brush: Theme.Muted)); _list.Items.Add(item);
        }
        _count.Text = _files.Count == 0 ? T("No files selected", "Файлы пока не выбраны")
            : T($"Selected: {_files.Count}", $"Выбрано файлов: {_files.Count}");
        _start.IsEnabled = _files.Count > 0;
    }

    private static Button Button(string text, Action click, bool primary = false, string? glyph = null)
    {
        var button = new Button
        {
            Content = text, Padding = new Thickness(16, 9, 16, 9), FontSize = 13,
            Background = primary ? Theme.Accent : Theme.Card,
            Foreground = primary ? Theme.TextOnAccent : Theme.Text,
            BorderBrush = Theme.PanelBorder, Cursor = System.Windows.Input.Cursors.Hand,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        button.Template = (ControlTemplate)System.Windows.Markup.XamlReader.Parse("""
            <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" TargetType="Button">
              <Border x:Name="Frame" Background="{TemplateBinding Background}" BorderBrush="{TemplateBinding BorderBrush}"
                      BorderThickness="1" CornerRadius="8" Padding="{TemplateBinding Padding}">
                <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />
              </Border>
              <ControlTemplate.Triggers>
                <Trigger Property="IsMouseOver" Value="True"><Setter TargetName="Frame" Property="Opacity" Value="0.85" /></Trigger>
                <Trigger Property="IsEnabled" Value="False"><Setter TargetName="Frame" Property="Opacity" Value="0.4" /></Trigger>
                <Trigger Property="IsKeyboardFocused" Value="True"><Setter TargetName="Frame" Property="BorderThickness" Value="2" /></Trigger>
              </ControlTemplate.Triggers>
            </ControlTemplate>
            """);
        button.Click += (_, _) => click();
        return button;
    }
}
