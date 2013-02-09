namespace CGO.Domain
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
    }
}
