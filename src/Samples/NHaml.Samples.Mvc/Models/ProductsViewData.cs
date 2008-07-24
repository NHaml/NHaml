using System.Web.Mvc;

using NHaml.Samples.Mvc.Models;

namespace NHaml.Samples.Mvc.Models
{
  public class ProductsEditViewData
  {
    public Product Product { get; set; }
    public SelectList Suppliers { get; set; }
    public SelectList Categories { get; set; }
  }

  public class ProductsNewViewData
  {
    public SelectList Suppliers { get; set; }
    public SelectList Categories { get; set; }
  }
}