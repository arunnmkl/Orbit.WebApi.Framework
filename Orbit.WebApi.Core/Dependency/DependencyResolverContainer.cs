using System;
using System.Collections.Generic;
using System.Linq;

namespace Orbit.WebApi.Core.Dependency
{
    /// <summary>
    /// Class that DependencyResolverContainer.
    /// </summary>
    public static class DependencyResolverContainer
    {
        /// <summary>
        /// This stores the mapping of the type and object.
        /// </summary>
        private static readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

        /// <summary>
        /// This stores the mapping of the type and object.
        /// </summary>
        private static readonly Dictionary<Type, Type> _mapType = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="T">Which type of instance do you want to keep and resolve.</typeparam>
        /// <param name="instance">The instance, this basically the actual object.</param>
        /// <remarks>For better use the type must be an interface.</remarks>
        public static void RegisterInstance<T>(object instance)
        {
            Type type = typeof(T);
            if (!_map.ContainsKey(type))
            {
                _map.Add(type, instance);
            }
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="T">This is type of the object to me stored</typeparam>
        /// <typeparam name="R">This is the class type to be resolved</typeparam>
        public static void Register<T, R>() where R : class, new()
        {
            Type type = typeof(T);
            if (!_mapType.ContainsKey(type))
            {
                _mapType.Add(type, typeof(R));
            }
        }

        /// <summary>
        /// Resolves the instance.
        /// </summary>
        /// <typeparam name="T">The type you want to resolve</typeparam>
        /// <returns>This returns the actual object</returns>
        public static T ResolveInstance<T>()
        {
            Type type = typeof(T);
            if (_mapType.ContainsKey(type))
            {
                try
                {
                    return (T)Activator.CreateInstance(_mapType[type]);
                }
                catch (Exception)
                {
                }
            }

            return default(T);
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T">Which type of object do you want.</typeparam>
        /// <returns>Returns the actual object as the given type.</returns>
        /// <remarks>For better use the type must be an interface.</remarks>
        public static T Resolve<T>()
        {
            Type type = typeof(T);
            if (_map.ContainsKey(type))
            {
                return (T)_map[type];
            }

            return default(T);
        }

        /// <summary>
        /// Returns true if type is valid.
        /// </summary>
        /// <typeparam name="T">The type which should be validated.</typeparam>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid<T>()
        {
            Type type = typeof(T);
            if (_map.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T">Which type of object do you want.</typeparam>
        /// <returns>Returns the actual object as the given type.</returns>
        /// <remarks>For better use the type must be an interface.</remarks>
        public static IEnumerable<T> ResolveAll<T>()
        {
            Type type = typeof(T);
            if (_map.ContainsKey(type))
            {
                return (IEnumerable<T>)_map.Where(obj => obj.Key.Equals(type)).Select(i => i.Value);
            }

            return new List<T>();
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object Resolve(Type type)
        {
            if (_map.ContainsKey(type))
            {
                return _map[type];
            }

            return null;
        }

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;System.Object&gt;.</returns>
        public static IEnumerable<object> ResolveAll(Type type)
        {
            if (_map.ContainsKey(type))
            {
                return _map.Where(obj => obj.Key.Equals(type)).Select(i => i.Value);
            }

            return new List<object>();
        }
    }
}