﻿using AniMate_app.Model;
using AniMate_app.Services.AnilibriaService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AniMate_app.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<GenreCollection> _genreList = new();

        public List<string> Genres { get; private set; }

        public int GenresLoaded { get; private set; } = 0;

        public bool AllGenresLoaded => GenresLoaded.Equals(Genres.Count);

        [ObservableProperty]
        private bool _isLoadingGenres = false;

        [ObservableProperty]
        private bool _isLoadingTitles = false;

        private readonly int _loadMoreGenresCount = 4;

        private readonly int _loadTitlesCount = 4;

        [ObservableProperty]
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
            IsBusy = true;

            Genres = await _anilibriaService.GetAllGenres();

            await LoadMoreGenres(_loadMoreGenresCount).ConfigureAwait(false);

            IsBusy = false;
        }

        [RelayCommand]
        public async Task Refresh()
        {
            if (IsBusy) return;

            IsBusy = true;

            IsRefreshing = true;

            GenreList.Clear();

            GenresLoaded = 0;

            await LoadMoreGenres(_loadMoreGenresCount).ConfigureAwait(false);

            IsRefreshing = false;

            IsBusy = false;
        }

        [RelayCommand]
        public async Task LoadMoreGenres()
        {
            if (!IsBusy && !IsLoadingGenres && !AllGenresLoaded)
                await LoadMoreGenres(_loadMoreGenresCount);
        }

        //[RelayCommand]
        //public void LoadMoreTitlesForGenre(GenreCollection genreCollection)
        //{
        //    if (genreCollection.TargetTitleCount.Equals(genreCollection.TitleCount))
        //        _loadMoreTitlesCommandsQueue.Add(new(genreCollection, LoadMoreTitlesForGenreAction, CanLoadMoreTitlesForGenre));

        //    else if (_loadMoreTitlesCommandsQueue.CommandCount > 0)
        //        _loadMoreTitlesCommandsQueue.Clear();
        //}

        //private bool CanLoadMoreTitlesForGenre(GenreCollection genreCollection)
        //{
        //    return genreCollection.TargetTitleCount.Equals(genreCollection.TitleCount) && !IsLoadingTitles;
        //}

        //private async void LoadMoreTitlesForGenreAction(GenreCollection genreCollection)
        //{
        //    if (IsLoadingTitles)
        //        return;

        //    IsLoadingTitles = true;

        //    if (genreCollection.TargetTitleCount > genreCollection.TitleCount)
        //        return;

        //    genreCollection.TargetTitleCount += _loadTitlesCount;

        //    genreCollection.AddTitleList(await _anilibriaService.GetTitlesByGenre(genreCollection.GenreName, genreCollection.TitleCount, _loadTitlesCount));

        //    IsLoadingTitles = false;
        //}

        private async Task LoadMoreGenres(int count)
        {
            IsLoadingGenres = true;

            List<GenreCollection> newGenres = await LoadGenres(count);

            GenreList.AddRange(newGenres);

            IsLoadingGenres = false;
        }

        private async Task<List<GenreCollection>> LoadGenres(int count)
        {
            int newCount = GenresLoaded + count < Genres.Count ? GenresLoaded + count : Genres.Count;

            List<GenreCollection> list = new();

            for (int i = GenresLoaded; i < newCount; i++)
            {
                GenreCollection genreCollection = new(Genres[i]);

                genreCollection.AddTitleList(await _anilibriaService.GetTitlesByGenre(Genres[i], 0, _loadTitlesCount));

                genreCollection.TargetTitleCount = _loadTitlesCount;

                list.Add(genreCollection);

                GenresLoaded++;
            }

            return list;
        }
    }
}
