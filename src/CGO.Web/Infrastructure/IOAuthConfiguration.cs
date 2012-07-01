namespace CGO.Web.Infrastructure
{
    /// <summary>
    /// Defines properties for retrieving OAuth-related configuration information.
    /// </summary>
    public interface IOAuthConfiguration
    {
        /// <summary>
        /// Gets the URL for the remote OAuth Service from which an authentication token can be retrieved.
        /// </summary>
        string OauthTokenUrl { get; }

        /// <summary>
        /// Gets the URL within this web application that the remote OAuth service should call back with the
        /// results of the authentication request.
        /// </summary>
        string OauthCallbackUrl { get; }

        /// <summary>
        /// Gets the identifier string that identifies this web application to the remote OAuth service.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Gets the secret key, unique to this web application, used when talking to the remote OAuth service.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Gets the set of requested permissions over the authentication request. The set of valid values will 
        /// vary by OAuth provider.
        /// </summary>
        string OauthGrantType { get; }

        /// <summary>
        /// Gets the URL on the remote OAuth service from which user profile information can be retrieved after
        /// a successful authentication attempt.
        /// </summary>
        string UserProfileUrl { get; }

        /// <summary>
        /// Gets the URL for the remote OAuth service to which authentication requests should be sent.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>A URL that can be used to make requests to the remote OAuth service.</returns>
        string MakeLoginUri(string state);
    }
}