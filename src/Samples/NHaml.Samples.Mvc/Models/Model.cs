using NHaml.Web.Mvc;

namespace NHaml.Samples.Mvc.Models
{
  public abstract class Model : IModel
  {
    public abstract int Id { get; }

    public bool IsNew
    {
      get { return Id == 0; }
    }
  }
}