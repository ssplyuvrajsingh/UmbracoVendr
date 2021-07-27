using System.Collections.Generic;

public class ProductModel
{
    public int Action { get; set; }
    public int MemberId { get; set; }
    public string ProductName { get; set; }
    public int ProductTypeId { get; set; }
    public string ProductTypeName { get; set; }
    public int ProductPrice { get; set; }
    public string ProductDescription { get; set; }
    public string ProductFiles { get; set; }
    public string ProductLogoLarge { get; set; }
    public string ProductLogoSmall { get; set; }
    public string ProductScreenshot { get; set; }
    public string ProductScreenshotDescription { get; set; }
    public string ProductStatus { get; set; }
}

public class ProductRootModel
{
    public List<ProductModel> ProductModel { get; set; }
}