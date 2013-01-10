using System.Collections.Generic;

namespace NHaml.Samples.Mvc.CSharp.Models
{
    public class Category
    {
        public Category() { }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public IList<Product> Products { get; set; }
    }

}