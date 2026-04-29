using System.Collections.Generic;

public class ContentsSettingsSequenceItemInfo
{
    #region Properties
    private ContentsStoreItemType _type;
    public ContentsStoreItemType type
    {
        get
        {
            return _type;
        }
    }
    #endregion

    #region Methods
    public ContentsSettingsSequenceItemInfo(ContentsStoreItemType type)
    {
        _type = type;
    }

    public override bool Equals(object obj)
    {
        ContentsSettingsSequenceItemInfo first = this;
        ContentsSettingsSequenceItemInfo second = (ContentsSettingsSequenceItemInfo)obj;

        return first == obj;
    }

    public static bool operator ==(ContentsSettingsSequenceItemInfo a, ContentsSettingsSequenceItemInfo b)
    {
        if ((object)a == (object)b)
            return true;
        else if ((object)a != null && (object)b != null)
            return a._type == b._type;
        else
            return false;
    }

    public static bool operator !=(ContentsSettingsSequenceItemInfo a, ContentsSettingsSequenceItemInfo b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return _type.GetHashCode();
    }
    #endregion
}
