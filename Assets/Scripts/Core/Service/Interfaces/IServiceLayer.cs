using System;

namespace Core.Service.Interfaces
{
    [Obsolete]
    public interface IServiceLayer
    {
        T GetService<T>(string id = null) where T : IService;
    }
}