using System;

namespace CGO.Domain.Services.Application
{
    public interface IDateTimeProvider 
    {
        DateTime Now { get; }
    }
}