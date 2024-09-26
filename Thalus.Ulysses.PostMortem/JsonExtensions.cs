using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Thalus.Ulysses.PostMortem
{
    public static class JsonExtensions
    {
        public static string? ToJson(this object obj, bool indent = false) 
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string)
            {
                return (string)obj;
            }
            return JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
        }

        public static TType? ToObjectOf<TType>(this string text)
        {
            if (text == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TType>(text);
        }
    }
}
