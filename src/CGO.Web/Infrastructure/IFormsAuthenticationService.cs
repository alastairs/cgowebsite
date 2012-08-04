using System.Web.Security;

namespace CGO.Web.Infrastructure
{
    /// <summary>
    /// Wrapper interface for <see cref="FormsAuthentication"/>.  
    /// </summary>
    public interface IFormsAuthenticationService
    {
        /// <summary>
        /// Creates an authentication ticket for the supplied user name and adds it to the cookies collection of the response, 
        /// or to the URL if you are using cookieless authentication.
        /// </summary>
        /// <param name="username">The name of an authenticated user. This does not have to map to a Windows account. </param>
        /// <param name="persistent"><c>true</c> to create a persistent cookie (one that is saved across browser sessions); otherwise, <c>false</c>. </param>
        void SetAuthenticationCookie(string username, bool persistent);

        /// <summary>
        /// Clears the authentication cookie to log the user out of the site.
        /// </summary>
        void SignOut();
    }
}