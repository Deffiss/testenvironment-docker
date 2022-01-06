using System;

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
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
