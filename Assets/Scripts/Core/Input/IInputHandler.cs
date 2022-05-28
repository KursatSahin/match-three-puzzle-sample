using Core.Service.Interfaces;

namespace Core.Input
{
    public interface IInputHandler : IService, IInitializeService, ITearDownService
    {
        
    }
}