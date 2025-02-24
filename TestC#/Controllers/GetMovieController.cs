using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestC_.Models;

namespace TestC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMovieController : ControllerBase
    {
        private static readonly List<Models.Movie> Movies = new List<Models.Movie>
        {
          new Models.Movie { mov_id =901,mov_title ="Vertigo",mov_year=1958,mov_time=128,mov_lang="English",mov_dt_rel=1958,mov_rel_country="UK" },
          new Models.Movie { mov_id =902,mov_title ="The Innocents ",mov_year=1961,mov_time=100,mov_lang="English",mov_dt_rel=1958,mov_rel_country="SW" },
          new Models.Movie { mov_id =903,mov_title ="Lawrence of Arabia ",mov_year=1962,mov_time=216,mov_lang="English",mov_dt_rel=1962,mov_rel_country="UK" },
          new Models.Movie { mov_id =904,mov_title ="The Deer Hunter ",mov_year=1978,mov_time=183,mov_lang="English",mov_dt_rel=1979,mov_rel_country="UK" },
        };
        private static readonly List<Models.Actor> Actors = new List<Models.Actor>
        {
         new Models.Actor{ act_id =101,act_fname="James",act_lname="Stewart",act_gender="M"},
         new Models.Actor{ act_id =102,act_fname="Deborah",act_lname="Kerr",act_gender="F"},   
         new Models.Actor{ act_id =103,act_fname="Peter",act_lname="OToole",act_gender="M"},   
         new Models.Actor{ act_id =104,act_fname="Robert",act_lname="De Niro ",act_gender="M"},

        };
        private static readonly List<Models.Movie_cast> Movie_casts = new List<Models.Movie_cast>
        {
         new Models.Movie_cast{ act_id =101,mov_id=901,role="John Scottie Ferguson"},
         new Models.Movie_cast{ act_id =102,mov_id=902,role="Miss Giddens"},
         new Models.Movie_cast{ act_id =103,mov_id=903,role="T.E. Lawrence"},
         new Models.Movie_cast{ act_id =104,mov_id=904,role="Michael"}
        };
        private static readonly List<Models.Director> Directors = new List<Models.Director>
        {
         new Models.Director{ dir_id =201,dir_fname="Alfred",dir_lname="Hitchcock"},
         new Models.Director{ dir_id =202,dir_fname="Jack",dir_lname="Clayton"},
         new Models.Director{ dir_id =203,dir_fname="David",dir_lname="Lean"},
         new Models.Director{ dir_id =204,dir_fname="Michael",dir_lname="Cimino"}

        };
        private static readonly List<Models.Movie_direction> Movie_directions = new List<Models.Movie_direction>
        {
         new Models.Movie_direction{ dir_id =201,mov_id=901},
         new Models.Movie_direction{ dir_id =202,mov_id=902},
         new Models.Movie_direction{ dir_id =203,mov_id=903},
         new Models.Movie_direction{ dir_id =204,mov_id=904}

        };

        [HttpGet("GetMovieInformation")]
        public IActionResult GetMovieInformation([FromQuery] string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Search term cannot be empty");
            }
            var matchingActors = Actors.Where(a => a.act_fname.Contains(name, System.StringComparison.OrdinalIgnoreCase) || a.act_lname.Contains(name, System.StringComparison.OrdinalIgnoreCase)).ToList();
            var movieIdsFromActors = Movie_casts.Where(mc => matchingActors.Any(a => a.act_id == mc.act_id)).Select(mc => mc.mov_id).Distinct().ToList();
            var matchingDirector = Directors.Where(d => d.dir_fname.Contains(name, System.StringComparison.OrdinalIgnoreCase) || d.dir_lname.Contains(name, System.StringComparison.OrdinalIgnoreCase)).ToList();
            var moviesFromDirectors = Movie_directions.Where(md => matchingDirector.Any(d => d.dir_id == md.dir_id)).Select(md => md.mov_id).Distinct().ToList();
            var results = Movies.Where(m => m.mov_title.Contains(name, System.StringComparison.OrdinalIgnoreCase) || movieIdsFromActors.Contains(m.mov_id)
            || moviesFromDirectors.Contains(m.mov_id)).Select(m => new { m.mov_id, m.mov_title, m.mov_time, m.mov_lang, m.mov_dt_rel, m.mov_rel_country, Cast = Movie_casts.Where(mc => mc.mov_id == m.mov_id).Select(mc => new { Actor = Actors.FirstOrDefault(a => a.act_id == mc.act_id), mc.role }).ToList(),Directors=Movie_directions.Where(md=>md.mov_id==m.mov_id).Select(md=>Directors.FirstOrDefault(d=>d.dir_id==md.dir_id)).ToList() });
            if(!results.Any())
            {
                return NotFound("No movie found");
            }
            return Ok(results);
        }

    }
}
