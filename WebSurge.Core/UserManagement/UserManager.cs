using System.Collections.Generic;
using Westwind.Utilities;

namespace WebSurge
{

    /// <summary>
    /// Class used to load and save User information from a file or string on disk
    /// </summary>
    public class UserManager
    {
        public static List<UserEntry> LoadUsersFromJsonFile(string filename)
        {
            object result = JsonSerializationUtils.DeserializeFromFile(filename,typeof(List<UserEntry>),throwExceptions: true);
            return result as List<UserEntry>;
        }

        public static List<UserEntry> LoadUsersFromJson(string json)
        {
            object result = JsonSerializationUtils.Deserialize(json, typeof(List<UserEntry>), throwExceptions: true);
            return result as List<UserEntry>;
        }


        public static bool SaveUsersToJsonFile(List<UserEntry> users, string filename)
        {
            return JsonSerializationUtils.SerializeToFile(users, filename, throwExceptions: true,formatJsonOutput: true);
        }


        public static string SaveUsersToJsonString(List<UserEntry> users)
        {
            return JsonSerializationUtils.Serialize(users, throwExceptions: true, formatJsonOutput: true);
        }
    }
}