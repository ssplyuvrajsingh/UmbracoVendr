using System.Collections.Generic;
using Umbraco.Web.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
public class ProvidersModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StreetName { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string TelephoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ProviderType { get; set; }
    public string ProviderStatus { get; set; }
    public string IdDocument { get; set; }
}

public class ProviderRootModel
{
    public List<ProvidersModel> ProvidersModel { get; set; }
    public RegisterModel RegisterModel { get; set; }
}