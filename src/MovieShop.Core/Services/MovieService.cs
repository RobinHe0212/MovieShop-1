﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;

namespace MovieShop.Core.Services
{
    public class MovieService: IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }
        public async Task<IEnumerable<Movie>> GetAllMoviesAsync(int pageSize = 20, int pageIndex = 0, string title = "")
        {
            throw new System.NotImplementedException();
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            return movie;
        }

        public async Task<int> GetMoviesCount(string title = "")
        {
            if (string.IsNullOrEmpty(title)) return await _movieRepository.GetCountAsync();
            return await _movieRepository.GetCountAsync(m => m.Title.Contains(title));
        }

        public async Task<IEnumerable<Movie>> GetTopRatedMovies()
        {
            return await _movieRepository.GetTopRatedMovies();
        }

        public async Task<IEnumerable<Movie>> GetHighestGrossingMovies()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId)
        {
            throw new System.NotImplementedException();
        }
    }
}