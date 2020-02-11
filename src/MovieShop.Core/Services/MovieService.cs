﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.Exceptions;
using MovieShop.Core.Helpers;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;

namespace MovieShop.Core.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;
        private readonly IAsyncRepository<MovieGenre> _genresRepository;

        private ICurrentUserService _currentUserService;

        public MovieService(IMovieRepository movieRepository, IMapper mapper, ICurrentUserService currentUserService, IAsyncRepository<MovieGenre> genresRepository)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _genresRepository = genresRepository;
        }


        public async Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(
            int pageSize = 20, int pageIndex = 0, string title = "")
        {
            Expression<Func<Movie, bool>> filterExpression = null;
            if (!string.IsNullOrEmpty(title)) filterExpression = movie => title != null && movie.Title.Contains(title);

            var pagedMovies = await _movieRepository.GetPagedData(pageIndex, pageSize,
                                                                  movies => movies.OrderBy(m => m.Title),
                                                                  filterExpression);
            var movies =
                new PagedResultSet<MovieResponseModel>(_mapper.Map<List<MovieResponseModel>>(pagedMovies),
                                                       pagedMovies.PageIndex,
                                                       pageSize, pagedMovies.TotalCount);
            return movies;
        }

        public async Task<MovieDetailsResponseModel> GetMovieAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) throw new NotFoundException("Movie", id);
            var response = _mapper.Map<MovieDetailsResponseModel>(movie);
            return response;
        }


        public async Task<int> GetMoviesCount(string title = "")
        {
            if (string.IsNullOrEmpty(title)) return await _movieRepository.GetCountAsync();
            return await _movieRepository.GetCountAsync(m => m.Title.Contains(title));
        }

        public async Task<IEnumerable<MovieResponseModel>> GetTopRatedMovies()
        {
            var topMovies = await _movieRepository.GetTopRatedMovies();
            var response = _mapper.Map<IEnumerable<MovieResponseModel>>(topMovies);
            return response;
        }

        public async Task<IEnumerable<MovieResponseModel>> GetHighestGrossingMovies()
        {
            var movies = await _movieRepository.GetHighestGrossingMovies();
            var response = _mapper.Map<IEnumerable<MovieResponseModel>>(movies);
            return response;
        }

        public async Task<IEnumerable<MovieResponseModel>> GetMoviesByGenre(int genreId)
        {
            var movies = await _movieRepository.GetMoviesByGenre(genreId);
            if (!movies.Any()) throw new NotFoundException("Movies for genre", genreId);
            var response = _mapper.Map<IEnumerable<MovieResponseModel>>(movies);
            return response;
        }

        public async Task<MovieDetailsResponseModel> CreateMovie(MovieCreateRequest movieCreateRequest)
        {
            //if (_currentUserService.UserId != favoriteRequest.UserId)
            //    throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to purchase");

            // check whether the user is Admin and can create the movie claim

            var movie = _mapper.Map<Movie>(movieCreateRequest);

            var createdMovie = await _movieRepository.AddAsync(movie);
           // var movieGenres = new List<MovieGenre>();
            foreach (var genre in movieCreateRequest.Genres)
            {
                var movieGenre = new MovieGenre {MovieId = createdMovie.Id, GenreId = genre.Id};
                await _genresRepository.AddAsync(movieGenre);
            }
            
            return _mapper.Map<MovieDetailsResponseModel>(createdMovie);
        }
    }
}