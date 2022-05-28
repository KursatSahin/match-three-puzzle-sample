namespace Core.Service.Interfaces
{
    public interface IService
    {
        
    }
    
    public interface ITearDownService
    {   
        /// <summary>
        /// Calls TearDown() on all ITearDownService instances
        /// </summary>
        void TearDown();
    }
    
    public interface IInitializeService
    {
        /// <summary>
        /// Calls Initialize() on all IInitializeService instances
        /// </summary>
        void Initialize();
    }
}