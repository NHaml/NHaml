using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.NHaml.Crosscutting;
using NUnit.Framework;

namespace NHaml.Tests.Crosscutting
{
    [TestFixture]
    public class Invariant_Tests
    {
        #region ArgumentNotNull Tests
        [Test]
        public void ArgumentNotNull_NonNullArgument_DoesNothing()
        {
            Assert.DoesNotThrow(() => Invariant.ArgumentNotNull(new object(), "param"));
        }

        [Test]
        public void ArgumentNotNull_NullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Invariant.ArgumentNotNull(null, "param"));
        }
        #endregion

        #region ArgumentNotEmpty Tests
        [Test]
        public void ArgumentNotEmpty_ValidString_DoesNothing()
        {
            Assert.DoesNotThrow(() => Invariant.ArgumentNotEmpty("Test", "param"));
        }

        [Test]
        public void ArgumentNotEmpty_NullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Invariant.ArgumentNotEmpty(null, "param"));
        }

        [Test]
        public void ArgumentNotEmpty_EmptyString_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Invariant.ArgumentNotEmpty(string.Empty, "param"));
        }
        #endregion
    }
}
