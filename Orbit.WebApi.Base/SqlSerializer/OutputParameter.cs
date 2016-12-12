using System.Data;

namespace Orbit.WebApi.Base.SqlSerializer
{
    public class OutputParameter<T> : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameter&lt;T&gt;"/> class with a direction of output
        /// </summary>
        /// <param name="name">The name.</param>
        public OutputParameter(string name)
            : base(name, default(T), ParameterDirection.Output, typeof(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameter&lt;T&gt;"/> class with a direction of Input/Output
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The input value.</param>
        public OutputParameter(string name, T value)
            : base(name, value, ParameterDirection.InputOutput, typeof(T))
        {
        }
    }
}