using Message.Domain.MessageAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Domain.Events;

public class RequestCancelledDomainEvent : INotification
{
    public CommonMessage Common { get; private set; }
    public string Identifier { get; private set; }
    
    public RequestCancelledDomainEvent(CommonMessage common, string identifier)
    {
        Common = common;
        Identifier = identifier;
    }
}
