using System;

namespace MegaDTelegramRemoteControl.Infrastructure.Models
{
    public class OperationException : Exception
    {
        public OperationException()
        {
        }

        public OperationException(string message)
            : base(message)
        {
        }

        public OperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}