using System;
using System.Collections.Generic;

/// <summary>
/// The content item information of the contents store.
/// </summary>
public class ContentsStoreItemInfo : ICloneable
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

    private double _price = 0.0;
    public double price
    {
        get
        {
            return _price;
        }
    }

    private string _name = "";
    public string name
    {
        get
        {
            return _name;
        }
    }

    private int _seq = -1;
    public int seq
    {
        get
        {
            return _seq;
        }
    }

    private string _description = "";
    public string description
    {
        get
        {
            return _description;
        }
    }

    private string _descriptionShort = "";
    public string descriptionShort
    {
        get
        {
            return _descriptionShort;
        }
    }

    private string _regName = "";
    public string regName
    {
        get
        {
            return _regName;
        }
    }

    public bool useYn
    {
        get;
        set;
    }

    private bool buyYn
    {
        get;
        set;
    }

    private System.DateTime _regDate = new System.DateTime(2011,1,1);
    public System.DateTime regDate
    {
        get
        {
            return _regDate;
        }
    }

    private string _version = "0.0";
    public string version
    {
        get
        {
            return _version;
        }
    }
    #endregion

    #region Methods
    public ContentsStoreItemInfo()
    {
    }
    public ContentsStoreItemInfo(ContentsStoreItemType newType, Dictionary<string, object> info)
    {
        buyYn = false;
        useYn = false;

        _type = newType;

        _Parse(info);
    }

    public ContentsStoreItemInfo(ContentsStoreItemType newType, BigboardContentMode mode)
    {
        _type = newType;
        _MakeData(mode);
    }

    public void ContentBuy()
    {
        buyYn = true;
    }

    public bool IsBuy()
    {
        return buyYn;
    }

    public void SetName(string str)
    {
        _name = str;
    }

    public void StandAloneDataParse(Dictionary<string, object> item)
    {
        if (item != null)
        {
            if (item.ContainsKey("name") && item["name"] != null)
                _name = item["name"].ToString();
            if (item.ContainsKey("description_short") && item["description_short"] != null)
                _descriptionShort = item["description_short"].ToString();
            if (item.ContainsKey("description_long") && item["description_long"] != null)
                _description = item["description_long"].ToString();
            if (item.ContainsKey("version") && item["version"] != null)
                _version = item["version"].ToString();
            if (item.ContainsKey("reg_date") && item["reg_date"] != null)
            {
                string regDateStr = item["reg_date"].ToString();
                var cultureInfo = new System.Globalization.CultureInfo("ko-KR", false);
                System.DateTime.TryParse(regDateStr, cultureInfo, System.Globalization.DateTimeStyles.None, out _regDate);
            }
            if (item.ContainsKey("use_yn") && item["use_yn"] != null)
                useYn = item["use_yn"].ToString() == "Y";
        }
    }

    private void _MakeData(BigboardContentMode mode)
    {
        _seq = (int)mode;

        _price = 0.00;
        _name = mode.ToString();
        _description = "LongDescription";
        _descriptionShort = "ShortDescription";
        _regName = "admin";
        _version = "0.0";
        ContentBuy();
        useYn = true;
    }

    private void _Parse(Dictionary<string, object> dict)
    {
        if(dict != null)
        {
            if (dict.ContainsKey("price") && dict["price"] != null)
                double.TryParse(dict["price"].ToString(), out _price);
            if (dict.ContainsKey("name") && dict["name"] != null)
                _name = dict["name"].ToString();
            if (dict.ContainsKey("seq") && dict["seq"] != null)
                int.TryParse(dict["seq"].ToString(), out _seq);
            if (dict.ContainsKey("description_long") && dict["description_long"] != null)
                _description = dict["description_long"].ToString().Replace("\\n", "\n");
            if (dict.ContainsKey("description_short") && dict["description_short"] != null)
                _descriptionShort = dict["description_short"].ToString();
            if (dict.ContainsKey("reg_name") && dict["reg_name"] != null)
                _regName = dict["reg_name"].ToString();
            if (dict.ContainsKey("reg_date") && dict["reg_date"] != null)
            {
                string regDateStr = dict["reg_date"].ToString();
                var cultureInfo = new System.Globalization.CultureInfo("ko-KR", false);
                System.DateTime.TryParse(regDateStr, cultureInfo, System.Globalization.DateTimeStyles.None, out _regDate);
            }
            if (dict.ContainsKey("version") && dict["version"] != null)
                _version = dict["version"].ToString();

            if (dict.ContainsKey("buy_yn") && dict["buy_yn"] != null)
            {
                string buy_yn = dict["buy_yn"].ToString();
                if (buy_yn.ToLower().Equals("y"))
                {
                    ContentBuy();
                }
            }
        }
    }

    public override bool Equals(object other)
    {
        ContentsStoreItemInfo first = (ContentsStoreItemInfo)this;
        ContentsStoreItemInfo second = (ContentsStoreItemInfo)other;

        return first == second;
    }

    public static bool operator ==(ContentsStoreItemInfo first, ContentsStoreItemInfo second)
    {
        if ((object)first == (object)second)
            return true;
        else if ((object)first != null && (object)second != null)
        {
            return (first.type == second.type) && (first.seq == second.seq);
        }
        else
            return false;
    }

    public static bool operator !=(ContentsStoreItemInfo first, ContentsStoreItemInfo second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return type.typeCode.GetHashCode() + seq;
    }


    object ICloneable.Clone()
    {
        return this.Clone();
    }

    public ContentsStoreItemInfo Clone()
    {
        return (ContentsStoreItemInfo)this.MemberwiseClone();
    }

    #endregion
}