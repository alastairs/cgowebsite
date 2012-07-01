using System;
using System.Web.Security;

namespace CGO.Web.Infrastructure
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SetAuthenticationCookie(string username, bool persistent)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username");
            }

            FormsAuthentication.SetAuthCookie(username, persistent);
        }
    }
}