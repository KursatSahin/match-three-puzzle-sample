using System;
using System.Collections.Generic;
using Core.Service.Interfaces;

namespace Core.Service
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;

        private readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>(); 

        private ServiceLocator()
        {
        }

        public void RegisterService<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
        }

        public T Get<T>() where T : IService
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                throw new Exception($"{type.Name} not registered.");
            }

            return (T) _services[type];
        }
    }
}