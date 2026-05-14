using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lightswitch.App;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DataContext = e.Parameter;
        base.OnNavigatedTo(e);
    }
}
