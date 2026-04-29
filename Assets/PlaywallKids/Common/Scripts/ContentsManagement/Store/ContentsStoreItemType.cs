using System;
using System.Collections.Generic;

/// <summary>
/// The type information of the contents store item.
/// </summary>
public class ContentsStoreItemType
{
    #region Enums
    public enum Mode
    {
        None = 0,
        Drawing2D,
        Drawing3D,
        Touch,
        Interaction,
        Motion,
        Aquarium,
        BubblePang,
        Coding
    }
    #endregion

    #region Properties
    private string _typeCode = "";
    public string typeCode
    {
        get
        {
            return _typeCode;
        }
    }

    private string _typeName = "";
    public string typeName
    {
        get
        {
            return _typeName;
        }
    }

    public Mode mode
    {
        get
        {
            return GetModeToTypeCode(_typeCode);
        }
    }
    #endregion

    #region Methods
    public ContentsStoreItemType(string code, string name)
    {
        _typeCode = code;
        _typeName = name;
    }

    public ContentsStoreItemType(Mode mode, string name)
    {
        _typeCode = GetTypeCodeToMode(mode);
        _typeName = name;
    }

    private string GetTypeCodeToMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Drawing2D: return "CC0101";
            case Mode.Drawing3D: return "CC0102";
            case Mode.Touch: return "CC0103";
            case Mode.Interaction: return "CC0104";
            case Mode.Motion: return "CC0105";
            case Mode.Aquarium: return "CC106";
        }
        return "";
    }

    private Mode GetModeToTypeCode(string code)
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Mode)).Length; i++)
            if (GetTypeCodeToMode((Mode)i).Equals(code))
                return (Mode)i;
        return Mode.None;
    }

    public override bool Equals(object other)
    {
        ContentsStoreItemType first = (ContentsStoreItemType)this;
        ContentsStoreItemType second = (ContentsStoreItemType)other;

        return first == second;
    }

    public static bool operator==(ContentsStoreItemType first, ContentsStoreItemType second)
    {
        if ((object)first == (object)second)
            return true;
        else if ((object)first != null && (object)second != null)
            return first.typeCode.Equals(second.typeCode);
        else
            return false;
    }

    public static bool operator !=(ContentsStoreItemType first, ContentsStoreItemType second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return typeCode.GetHashCode();
    }
    #endregion
}
