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

        /// <summary>
        /// Add the service as a type of T service to the services dictionary.
        /// </summary>
        /// <param name="service">Instance object of service</param>
        /// <typeparam name="T">Generic type service</typeparam>
        public void RegisterService<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
        }

        /// <summary>
        /// Returns service of type T if registered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception">Throws if requested service was not registered</exception>
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