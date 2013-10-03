using System.Collections.Generic;
using CGO.Domain.Entities;

namespace CGO.Domain.Services
{
    /// <summary>
    /// A service for retrieving details about one or more Concerts
    /// </summary>
    public interface IConcertDetailsService
    {
        /// <summary>
        /// Return a single Concert with the given identity
        /// </summary>
        /// <param name="concertId">The identifier for the requested Concert.</param>
        /// <returns></returns>
        Concert GetConcert(int concertId);

        /// <summary>
        /// Return all published concerts that occur in the future, ordered by ascending date.
        /// </summary>
        /// <returns>A collection of concerts in the published state.</returns>
        IReadOnlyCollection<Concert> GetFutureConcerts();

        /// <summary>
        /// Saves the specified concert to the backing store.
        /// </summary>
        /// <param name="concert">The concert to save.</param>
        void SaveConcert(Concert concert);
    }
}
