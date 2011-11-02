using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.IO;

namespace HamlSpec
{
    internal class HamlSpecLoader
    {
        public IEnumerable<HamlSpec> GetTheHamlSpecTests(string groupName)
        {
            var group = GetTestGroup(groupName);
            var hamlTests = GetDictionary(group);
            return hamlTests.Select(x => GetHamlSpec(groupName, x));
        }

        private object GetTestGroup(string groupName)
        {
            var json = File.ReadAllText(@"tests.json");

            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var deserializedObject = serializer.DeserializeObject(json);
            return ((IDictionary<string, object>)deserializedObject)[groupName];
        }

        private HamlSpec GetHamlSpec(string groupName, KeyValuePair<string, object> test)
        {
            var properties = GetDictionary(test.Value);
            return new HamlSpec
            {
                GroupName = groupName,
                TestName = test.Key,
                Haml = properties["haml"].ToString(),
                ExpectedHtml = properties["html"].ToString(),
                HasAConfigBlock = properties.ContainsKey("config")
            };
        }

        private static IDictionary<string, object> GetDictionary(object deserializedObject)
        {
            return (IDictionary<string, object>)deserializedObject;
        }
    }
}
