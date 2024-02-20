using AniMate_app.Services.AnilibriaService.Models;
using CommunityToolkit.Mvvm.ComponentModel;
namespace AniMate_app.ViewModels
{
    [QueryProperty(nameof(Title), "TheTitle")]
    public partial class TitleViewModel : ObservableObject
    {
        public Title _title;
        public Title Title
        {
            get => _title;
            set
            {
                _title = value;
                Genres = string.Join(", ", _title.Genres);
                ShortDescription = string.Join(" ", _title.RuDescription.Split(' ').Take(7));
                OnPropertyChanged(nameof(Title));
            }
        }

        [ObservableProperty]
        private List<Episode> _episodeButtons = new();

        [ObservableProperty]
        private string _genres;

        [ObservableProperty]
        private string _shortDescription;

        public async Task CreateEpisodeButtons()
        {
            EpisodeButtons.Clear();

            foreach(var episode in Title.Player.Episodes.Values)
            {
                EpisodeButtons.Add(episode);              
            }
        }
    }
}
