using System.IO;
using TestC_.Interface;
using TestC_.Models;

namespace TestC_.Services
{
    public class MovieService : IMovieService
    {
        private readonly List<Movie> _movies;
        private readonly List<Actor> _actors;
        private readonly List<Movie_cast> _movieCasts;
        private readonly List<Director> _directors;
        private readonly List<Movie_direction> _movieDirections;

        public MovieService()
        {
            _movies = new List<Movie>
            {
                new Movie { mov_id = 901, mov_title = "Vertigo", mov_year = 1958, mov_time = 128, mov_lang = "English", mov_dt_rel = 1958, mov_rel_country = "UK" },
                new Movie { mov_id = 902, mov_title = "The Innocents", mov_year = 1961, mov_time = 100, mov_lang = "English", mov_dt_rel = 1958, mov_rel_country = "SW" },
                new Movie { mov_id = 903, mov_title = "Lawrence of Arabia", mov_year = 1962, mov_time = 216, mov_lang = "English", mov_dt_rel = 1962, mov_rel_country = "UK" },
                new Movie { mov_id = 904, mov_title = "The Deer Hunter", mov_year = 1978, mov_time = 183, mov_lang = "English", mov_dt_rel = 1979, mov_rel_country = "UK" }
            };

            _actors = new List<Actor>
            {
                new Actor { act_id = 101, act_fname = "James", act_lname = "Stewart", act_gender = "M" },
                new Actor { act_id = 102, act_fname = "Deborah", act_lname = "Kerr", act_gender = "F" },
                new Actor { act_id = 103, act_fname = "Peter", act_lname = "O'Toole", act_gender = "M" },
                new Actor { act_id = 104, act_fname = "Robert", act_lname = "De Niro", act_gender = "M" }
            };

            _movieCasts = new List<Movie_cast>
            {
                new Movie_cast { act_id = 101, mov_id = 901, role = "John Scottie Ferguson" },
                new Movie_cast { act_id = 102, mov_id = 902, role = "Miss Giddens" },
                new Movie_cast { act_id = 103, mov_id = 903, role = "T.E. Lawrence" },
                new Movie_cast { act_id = 104, mov_id = 904, role = "Michael" }
            };

            _directors = new List<Director>
            {
                new Director { dir_id = 201, dir_fname = "Alfred", dir_lname = "Hitchcock" },
                new Director { dir_id = 202, dir_fname = "Jack", dir_lname = "Clayton" },
                new Director { dir_id = 203, dir_fname = "David", dir_lname = "Lean" },
                new Director { dir_id = 204, dir_fname = "Michael", dir_lname = "Cimino" }
            };

            _movieDirections = new List<Movie_direction>
            {
                new Movie_direction { dir_id = 201, mov_id = 901 },
                new Movie_direction { dir_id = 202, mov_id = 902 },
                new Movie_direction { dir_id = 203, mov_id = 903 },
                new Movie_direction { dir_id = 204, mov_id = 904 }
            };
        }

        public List<object> SearchMovies(string name)
        {
            try
            {
                var matchingActors = _actors
                    .Where(a => a.act_fname.Contains(name, StringComparison.OrdinalIgnoreCase)
                             || a.act_lname.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var movieIdsFromActors = _movieCasts
                    .Where(mc => matchingActors.Any(a => a.act_id == mc.act_id))
                    .Select(mc => mc.mov_id)
                    .Distinct()
                    .ToList();

                var matchingDirectors = _directors
                    .Where(d => d.dir_fname.Contains(name, StringComparison.OrdinalIgnoreCase)
                             || d.dir_lname.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var moviesFromDirectors = _movieDirections
                    .Where(md => matchingDirectors.Any(d => d.dir_id == md.dir_id))
                    .Select(md => md.mov_id)
                    .Distinct()
                    .ToList();

                var results = _movies
                    .Where(m => m.mov_title.Contains(name, StringComparison.OrdinalIgnoreCase)
                             || movieIdsFromActors.Contains(m.mov_id)
                             || moviesFromDirectors.Contains(m.mov_id))
                    .Select(m => new
                    {
                        m.mov_id,
                        m.mov_title,
                        m.mov_time,
                        m.mov_lang,
                        m.mov_dt_rel,
                        m.mov_rel_country,
                        Cast = _movieCasts
                            .Where(mc => mc.mov_id == m.mov_id)
                            .Select(mc => new
                            {
                                Actor = _actors.FirstOrDefault(a => a.act_id == mc.act_id),
                                mc.role
                            })
                            .ToList(),
                        Directors = _movieDirections
                            .Where(md => md.mov_id == m.mov_id)
                            .Select(md => _directors.FirstOrDefault(d => d.dir_id == md.dir_id))
                            .ToList()
                    })
                    .ToList<object>(); // Convert to List<object> as the return type expects it.

                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching movies", ex);
            }
        }

    }
}
