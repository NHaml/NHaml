using System.Collections.Generic;
using System.Linq;

namespace NHaml.Samples.Mvc.Boo.Models
{
    public partial class NorthwindDataContext
    {
        // Retrieve All Category Objects

        public List<Category> GetCategories()
        {
            return Categories.ToList();
        }

        // Retrieve all Suppliers

        public List<Supplier> GetSuppliers()
        {
            return Suppliers.ToList();
        }

        // Retrieve a Specific Category

        public Category GetCategoryById( int categoryId )
        {
            return Categories.First( c => c.CategoryID == categoryId );
        }

        // Retrieve a specific Product

        public Product GetProductById( int productId )
        {
            return Products.Single( p => p.ProductId == productId );
        }

        // Add a New Product

        public void AddProduct( Product product )
        {
            Products.InsertOnSubmit( product );
        }
    }
}