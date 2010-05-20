using System.Collections.Generic;

namespace NHaml.Core.Utils
{

    //TODO: remove when move to 3.5
    public static class ListExtensions
    {
        public static T Last<T>(IList<T> list)
        {
            return list[list.Count - 1];
        }
    }
}