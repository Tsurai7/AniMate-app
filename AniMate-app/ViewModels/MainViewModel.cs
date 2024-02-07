﻿using AniMate_app.Model;
using AniMate_app.Services.AnilibriaService;
using AniMate_app.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AniMate_app.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<GenreCollection> TitlesByGenre { get; private set; } = new();

        public List<string> Genres { get; private set; }

        public int GenresLoaded { get; private set; } = 0;

        public int RemainingItems { get; private set; }

        public bool AllGenresLoaded => GenresLoaded.Equals(Genres.Count);

        public bool IsTitlesLoaded { get; private set; } = true;

        public bool IsGenresLoaded { get; private set; } = true;

        private readonly CommandCollection<int> _loadGenreCommandsQueue = new();

        private readonly CommandCollection<GenreCollection> _loadMoreTitlesCommandsQueue = new();

        private Utils.Command<int> LoadNewGenreTitlesCommand => new(5, LoadTitlesByGenreAction, CanLoadMoreGenres);

        private readonly int _loadMoreTitlesCount = 5;

        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isRefreshing;

        public readonly AnilibriaService _anilibriaService;

        public MainViewModel(AnilibriaService anilibriaService)
        {
            _anilibriaService = anilibriaService;
        }

        public async Task LoadContent()
        {
            _loadGenreCommandsQueue.Clear();
            
            _loadMoreTitlesCommandsQueue.Clear();

            Genres = await _anilibriaService.GetAllGenres();

            RemainingItems = Genres.Count - GenresLoaded - 1;

            LoadGenres();

            _loadGenreCommandsQueue.Add(LoadNewGenreTitlesCommand);
        }

        [RelayCommand]
        public void Refresh()
        {
            if (_isBusy) return;

            _isBusy = true;

            IsRefreshing = true;

            _loadGenreCommandsQueue.Clear();

            _loadMoreTitlesCommandsQueue.Clear();

            TitlesByGenre.Clear();

            LoadGenres();

            GenresLoaded = 0;

            IsGenresLoaded = true;

            _loadGenreCommandsQueue.Add(LoadNewGenreTitlesCommand);

            IsRefreshing = false; 

            _isBusy = false;
        }

        private void LoadGenres()
        {
            foreach (var genre in Genres)
                TitlesByGenre.Add(new(genre));
        }

        [RelayCommand]
        public void LoadMoreGenres()
        {
            if (!AllGenresLoaded)
                _loadGenreCommandsQueue.Add(LoadNewGenreTitlesCommand);
            else if (_loadGenreCommandsQueue.CommandCount > 0)
                _loadGenreCommandsQueue.Clear();
        }

        [RelayCommand]
        public void LoadMoreTitlesForGenre(GenreCollection genreCollection)
        {
            if (genreCollection.TargetTitleCount.Equals(genreCollection.TitleCount))
                _loadMoreTitlesCommandsQueue.Add(new(genreCollection, LoadMoreTitlesForGenreAction, CanLoadMoreTitlesForGenre));

            else if (_loadMoreTitlesCommandsQueue.CommandCount > 0)
                _loadMoreTitlesCommandsQueue.Clear();
        }

        private bool CanLoadMoreTitlesForGenre(GenreCollection genreCollection)
        {
            return genreCollection.TargetTitleCount.Equals(genreCollection.TitleCount) && IsTitlesLoaded;
        }

        private async void LoadMoreTitlesForGenreAction(GenreCollection genreCollection)
        {
            if (!IsTitlesLoaded)
                return;

            IsTitlesLoaded = false;

            if (genreCollection.TargetTitleCount > genreCollection.TitleCount)
                return;

            genreCollection.TargetTitleCount += _loadMoreTitlesCount;

            genreCollection.AddTitleList(await _anilibriaService.GetAllTitlesByGenre(genreCollection.GenreName, genreCollection.TitleCount, _loadMoreTitlesCount));

            IsTitlesLoaded = true;
        }

        private bool CanLoadMoreGenres(int count)
        {
            return IsGenresLoaded && !AllGenresLoaded;
        }

        private async void LoadTitlesByGenreAction(int count)
        {
            if(!IsGenresLoaded) return;

            IsGenresLoaded = false;

            int newCount = GenresLoaded + count < Genres.Count ? GenresLoaded + count : Genres.Count;

            for (int i = GenresLoaded; i < newCount; i++)
            {
                var genreTitles = TitlesByGenre[i];

                genreTitles.AddTitleList(await _anilibriaService.GetAllTitlesByGenre(Genres[i], 0, 5));

                genreTitles.TargetTitleCount = count;
            }

            GenresLoaded = newCount;

            IsGenresLoaded = true;

            RemainingItems = Genres.Count - GenresLoaded - 1;

            OnPropertyChanged(nameof(RemainingItems));
        }
    }
}