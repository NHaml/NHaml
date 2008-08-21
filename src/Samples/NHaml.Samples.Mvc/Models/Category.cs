namespace NHaml.Samples.Mvc.Models
{
  public partial class Category : Model
  {
    public override int Id
    {
      get { return CategoryID; }
    }

    public override string ToString()
    {
      return CategoryName;
    }
  }
}