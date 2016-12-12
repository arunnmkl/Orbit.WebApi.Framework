using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Orbit.WebApi.Base.SqlSerializer
{
    public class SetCollection : DictionaryBase
    {
        /// <summary>
        /// The _set types
        /// </summary>
        private readonly List<Type> _setTypes = new List<Type>();

        /// <summary>
        /// The _set names
        /// </summary>
        private readonly List<String> _setNames = new List<String>();

        /// <summary>
        /// Gets the <see cref="IList"/> with the specified type.
        /// </summary>
        /// <value>
        /// The <see cref="IList"/>.
        /// </value>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IList this[Type type]
        {
            get
            {
                var index = _setTypes.IndexOf(type);
                var name = _setNames[index];
                return this[name];
            }
        }

        /// <summary>
        /// Gets the <see cref="IList"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="IList"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IList this[string name]
        {
            get { return (IList)Dictionary[name]; }
        }

        /// <summary>
        /// Gets the <see cref="IList"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="IList"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IList this[int index]
        {
            get
            {
                var name = _setNames[index];
                return this[name];
            }
        }

        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public List<dynamic> GetSet(string name)
        {
            return (List<dynamic>)this[name];
        }

        public List<T> GetSet<T>()
        {
            return GetSet<T, T>();
        }

        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public List<T> GetSet<T>(string name)
        {
            return (List<T>)this[name];
        }

        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <returns></returns>
        public List<TReturn> GetSet<TObject, TReturn>()
        {
            return (List<TReturn>)this[typeof(TObject)];
        }

        /// <summary>
        /// Adds the set.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddSet(string name = null)
        {
            AddSet<ExpandoObject, dynamic>(name);
        }

        /// <summary>
        /// Adds the set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        public void AddSet<T>(string name = null)
        {
            AddSet<T, T>(name);
        }

        /// <summary>
        /// Adds the set.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="name">The name.</param>
        public void AddSet<TObject, TReturn>(string name = null)
        {
            name = name ?? typeof(TObject).ToString();
            _setTypes.Add(typeof(TObject));
            _setNames.Add(name);
            Dictionary.Add(name, new List<TReturn>());
        }

        /// <summary>
        /// Gets the type of the set.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Type GetSetType(int index)
        {
            return _setTypes[index];
        }
    }
}