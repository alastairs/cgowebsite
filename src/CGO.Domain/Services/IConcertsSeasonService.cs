using System.Collections.Generic;
using CGO.Domain.Entities;

namespace CGO.Domain.Services
{
    /// <summary>
    /// Provides ways of looking up Concerts in particular seasons.  Seasons run
    /// (nominally) from 1 August - 31 July.  
    /// </summary>
    public interface IConcertsSeasonService
    {
        /// <summary>
        /// Retrieve all the Concerts in the current season. The current season
        /// is determined by the current date. 
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Concert> GetConcertsInCurrentSeason();

        /// <summary>
        /// Retrieve all the Concerts in the previous season. The previous season
        /// is determined by the current date.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<Concert> GetConcertsInPreviousSeason();

        /// <summary>
        /// Retrieve all the Concerts in the requested season.
        /// </summary>
        /// <param name="seasonStartYear">The year in which the season started. For 
        /// example, if the value 2010 is supplied, the season returned will be the
        /// 2010-11 season.</param>
        /// <returns></returns>
        IReadOnlyCollection<Concert> GetConcertsInSeason(int seasonStartYear);
    }
}
