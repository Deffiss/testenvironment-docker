using System;
using System.Runtime.Serialization;

namespace TestEnvironment.Docker.ContainerOperations
{
    [Serializable]
    public class ContainerOperationException : Exception
    {
        public ContainerOperationException()
        {
        }

        public ContainerOperationException(string message)
            : base(message)
        {
        }

        public ContainerOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ContainerOperationException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }
    }
}
