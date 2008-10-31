using System;
using System.Web.Mvc;

using Moq;

using NHaml.Engine;
using NHaml.Web.Mvc;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class CompiledViewCacheTests
  {
    private CompiledViewCache<INHamlMvcView> _cache;

    [SetUp]
    public void Setup()
    {
      _cache = new CompiledViewCache<INHamlMvcView>();
    }

    [Test]
    public void TemplateCompilerShouldBeInstanceOfTemplateCompiler()
    {
      Assert.IsInstanceOfType(typeof(TemplateCompiler), _cache.TemplateCompiler);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetViewShouldThrowWhenCreateCompiledViewDelegateIsNull()
    {
      _cache.GetView(null, "foo", () => null);
    }

    [Test, ExpectedException(typeof(ArgumentNullException))]
    public void GetViewShouldThrowWhenObtainViewDataDelegateIsNull()
    {
      _cache.GetView(() => null, "foo", null);
    }

    [Test]
    public void WhenViewIsNotCachedTheDelegateShouldBeInvokedToObtainTheView()
    {
      var compiledView = new Mock<ICompiledView<INHamlMvcView>>();
      var view = new Mock<INHamlMvcView>();
      compiledView.Expect(x => x.CreateView()).Returns(view.Object);

      var fromCache = _cache.GetView(() => compiledView.Object, "foo", () => typeof(NHamlMvcView<ViewDataDictionary>));

      Assert.AreSame(view.Object, fromCache);
    }

    [Test]
    public void ShouldReturnCachedViewWhenAlreadyCached()
    {
      var compiledView = new Mock<ICompiledView<INHamlMvcView>>();
      var view = new Mock<INHamlMvcView>();
      compiledView.Expect(x => x.CreateView()).Returns(view.Object);

      _cache.GetView(() => compiledView.Object, "foo", () => typeof(NHamlMvcView<ViewDataDictionary>));
      //View should now be cached

      var fromCache = _cache.GetView(
        () =>
          {
            throw new AssertionException("This delegate should not be invoked");
          },
        "foo",
        () => typeof(NHamlMvcView<ViewDataDictionary>));

      Assert.AreSame(view.Object, fromCache);
    }

    [Test]
    public void ShouldCallRecompileIfNecessaryWhenNotInProduction()
    {
      var compiledView = new Mock<ICompiledView<INHamlMvcView>>();
      var view = new Mock<INHamlMvcView>();
      compiledView.Expect(x => x.CreateView()).Returns(view.Object);

      _cache.TemplateCompiler.IsProduction = false;

      _cache.GetView(() => compiledView.Object, "foo", () => typeof(NHamlMvcView<ViewDataDictionary>));

      compiledView.Verify(x => x.RecompileIfNecessary());
    }

    [Test]
    public void ShouldNotCallRecompileIfNeccessaryWhenInProduction()
    {
      var compiledView = new Mock<ICompiledView<INHamlMvcView>>();
      var view = new Mock<INHamlMvcView>();
      compiledView.Expect(x => x.CreateView()).Returns(view.Object);
      compiledView.Expect(x => x.RecompileIfNecessary()).Throws(
        new AssertionException("RecompileIfNecessary should not be called"));

      _cache.TemplateCompiler.IsProduction = true;

      _cache.GetView(() => compiledView.Object, "foo", () => typeof(NHamlMvcView<ViewDataDictionary>));
    }
  }
}