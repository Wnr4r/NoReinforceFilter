using System;
using Newtonsoft.Json;

namespace CrabadaFilter.Common {
    public static class Utils {
        public static bool TryDeserializeObject<T>(this string obj, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(obj);
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }
    }
}
