using Newtonsoft.Json;
using System.Collections.Generic;

public class KinderRegistData
{
    [JsonProperty("loginId")]
    public string LoginId { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("kinderName")]
    public string KinderName { get; set; }

    [JsonProperty("kinderTel")]
    public string KinderTel { get; set; }

    [JsonProperty("kinderUniqueNo")]
    public string KinderUniqueNo { get; set; }

    [JsonProperty("macAddress")]
    public string MacAddress { get; set; }

    [JsonProperty("kinderAddress")]
    public string KinderAddress { get; set; }

    [JsonProperty("activateContentsGrpList")]
    public List<ActivateContentsGrp> ActivateContentsGrpList { get; set; }

    [JsonProperty("agencyId")]
    public string AgencyId { get; set; }
}


public class ActivateContentsGrp
{
    [JsonProperty("contentGrpId")]
    public string ContentGrpId { get; set; }

    [JsonProperty("licenseEndDate")]
    public string LicenseEndDate { get; set; }
}