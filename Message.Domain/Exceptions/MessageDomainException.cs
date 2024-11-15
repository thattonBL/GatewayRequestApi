using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Domain.Exceptions;

public class MessageDomainException : Exception
{
    public MessageDomainException()
    { }

    public MessageDomainException(string message)
        : base(message)
    { }

    public MessageDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
