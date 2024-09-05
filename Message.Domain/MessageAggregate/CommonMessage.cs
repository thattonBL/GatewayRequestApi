using Message.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Domain.MessageAggregate;

public class CommonMessage : Entity
{
    private string _msg_status;
    private string _msg_source;
    private string _prty;
    private string _ref_source;
    private string _ref_request_id;
    private int? _ref_seq_no;
    private DateTime _dt_created;
    private int _msg_target;
    private int _m_type;

    public int msg_target
    {
        get
        {
            return _msg_target;
        }
        set
        {
            _msg_target = value;
        }
    }

    public int m_type
    {
        get
        {
            return _m_type;
        }
        set
        {
            _m_type = value;
        }
    }

    public messageTypeLookup messageTypeLookup { get; set; }

    protected CommonMessage()
    {
        _msg_status = String.Empty;
        _msg_source = String.Empty;
        _msg_target = 0;
        _prty = String.Empty;
        _m_type = 0;
        _ref_source = String.Empty;
        _ref_request_id = String.Empty;
        _ref_seq_no = 0;
        _dt_created = new DateTime();
        messageTypeLookup = new messageTypeLookup(); // Initialize the property
    }

    public CommonMessage(string msgStatus, string msgSource, int msgTarget, string mPrty, int mType, string refSource, string refRequestId, int refSeqNo, DateTime dtCreated)
    {
        _msg_status = msgStatus;
        _msg_source = msgSource;
        _msg_target = msgTarget;
        _prty = mPrty;
        _m_type = mType;
        _ref_source = refSource;
        _ref_request_id = refRequestId;
        _ref_seq_no = refSeqNo;
        _dt_created = dtCreated;
        messageTypeLookup = new messageTypeLookup(); // Initialize the property
    }
}