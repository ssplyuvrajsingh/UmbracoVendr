using USNWebsite.USNModels;

public interface IProductSurfaceServices
{
    void UpdateProductDetails(string productId, string productStatus);
    USNCreateProductFormViewModel GetProductDetailsById(string productId, string calledFrom);
}