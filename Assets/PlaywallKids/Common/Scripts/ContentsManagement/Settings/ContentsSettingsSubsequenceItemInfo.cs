using System.Collections.Generic;

public class ContentsSettingsSubsequenceItemInfo
{
    #region Properties
    private int _seq = 0;
    public int seq
    {
        get
        {
            return _seq;
        }
    }

    private string _name = "";
    public string name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public bool useYn
    {
        get;
        set;
    }
    #endregion

    #region Methods
    public ContentsSettingsSubsequenceItemInfo(int seqNum)
    {
        _seq = seqNum;
    }

    public override bool Equals(object obj)
    {
        ContentsSettingsSubsequenceItemInfo first = this;
        ContentsSettingsSubsequenceItemInfo second = (ContentsSettingsSubsequenceItemInfo)obj;

        return first == obj;
    }

    public static bool operator ==(ContentsSettingsSubsequenceItemInfo first, ContentsSettingsSubsequenceItemInfo second)
    {
        if ((object)first == (object)second)
            return true;
        else if ((object)first != null && (object)second != null)
            return first.seq.Equals(second.seq);
        else
            return false;
    }

    public static bool operator !=(ContentsSettingsSubsequenceItemInfo first, ContentsSettingsSubsequenceItemInfo second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return _seq;
    }

    public ContentsSettingsSubsequenceItemInfo Clone()
    {
        ContentsSettingsSubsequenceItemInfo info = new ContentsSettingsSubsequenceItemInfo(seq);
        info.useYn = useYn;
        info.name = name;
        return info;
    }
    #endregion
}