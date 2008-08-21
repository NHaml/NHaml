namespace NHaml.Samples.Mvc.Models
{
  public partial class Product : Model
  {
    public override int Id
    {
      get { return ProductID; }
    }

    public override string ToString()
    {
      return ProductName;
    }
  }
}