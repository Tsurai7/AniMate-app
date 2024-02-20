using CommunityToolkit.Mvvm.ComponentModel;

namespace AniMate_app.ViewModels
{
    [QueryProperty(nameof(MediaUrl), "mediaurl")]
    public partial class PlayerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _mediaUrl;
    }
}
