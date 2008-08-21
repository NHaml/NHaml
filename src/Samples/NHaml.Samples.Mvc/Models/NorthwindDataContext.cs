using System.Collections.Generic;
using System.Linq;

namespace NHaml.Samples.Mvc.Models
{
  public partial class NorthwindDataContext
  {
    public List<Category> GetCategories()
    {
      return Categories.ToList();
    }

    public List<Supplier> GetSuppliers()
    {
      return Suppliers.ToList();
    }

    public Category GetCategoryById(int id)
    {
      return Categories.First(c => c.CategoryID == id);
    }

    public Product GetProductById(int id)
    {
      return Products.Single(p => p.ProductID == id);
    }

    public void AddCategory(Category category)
    {
      Categories.InsertOnSubmit(category);
    }

    public void RemoveCategory(int id)
    {
      Categories.DeleteOnSubmit(GetCategoryById(id));
    }

    public void AddProduct(Product product)
    {
      Products.InsertOnSubmit(product);
    }

    public void RemoveProduct(int id)
    {
      Products.DeleteOnSubmit(GetProductById(id));
    }
  }
}