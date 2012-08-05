namespace CGO.Web.Infrastructure
{
    public interface IUrlHelper
    {
        string Action(string actionName, string controllerName, object routeValues = null);
    }
}