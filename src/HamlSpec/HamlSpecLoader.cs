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
        private string fileName;

        public HamlSpecLoader(string fileName)
        {
            this.fileName = fileName;
        }
        public IDictionary<string, IEnumerable<HamlSpec>> GetTheHamlSpecTests()
        {
            var deserializedFile = GetDeserializedFile();
            var result = new Dictionary<string,IEnumerable<HamlSpec>>();

            foreach (var key in deserializedFile.Keys)
            {
                var entry = (IDictionary<string, object>)deserializedFile[key];
                result.Add(key, entry.Select(x => GetHamlSpec(key, x)));
            }

            return result;
        }

        private IDictionary<string, object> GetDeserializedFile()
        {
            var json = File.ReadAllText(fileName);
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return (IDictionary<string, object>)serializer.DeserializeObject(json);
        }

        private HamlSpec GetHamlSpec(string groupName, KeyValuePair<string, object> test)
        {
            var properties = GetDictionary(test.Value);
            var result = new HamlSpec
            {
                GroupName = groupName,
                TestName = test.Key,
                Haml = properties["haml"].ToString(),
                ExpectedHtml = properties["html"].ToString()
            };

            if (properties.ContainsKey("config"))
            {
                result.Format = GetDictionary(properties["config"])["format"].ToString();
            }

            return result;
        }

        private static IDictionary<string, object> GetDictionary(object deserializedObject)
        {
            return (IDictionary<string, object>)deserializedObject;
        }
    }
}
