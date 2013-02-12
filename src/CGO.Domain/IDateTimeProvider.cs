using System;

namespace CGO.Domain
{
    public interface IDateTimeProvider 
    {
        DateTime Now { get; }
    }
}