using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Spaghetti.Face.Views;
using Spaghetti.Face.ViewModels;

namespace Spaghetti;

sealed partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      // OxyPlot
      desktop.MainWindow = new OxyPlotView()
      {
        DataContext = new OxyPlotViewModel()
      };

      // ScottPlot
      //desktop.MainWindow = new ScottPlotView()
      //{
      //  DataContext = new ScottPlotViewModel()
      //};

      // Skia
      //desktop.MainWindow = new SkiaView()
      //{
      //  DataContext = new SkiaViewModel()
      //};
    }

    base.OnFrameworkInitializationCompleted();
  }
}
