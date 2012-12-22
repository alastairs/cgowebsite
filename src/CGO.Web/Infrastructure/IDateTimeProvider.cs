using System;

namespace CGO.Web.Infrastructure
{
    public interface IDateTimeProvider 
    {
        DateTime Now { get; }
    }
}