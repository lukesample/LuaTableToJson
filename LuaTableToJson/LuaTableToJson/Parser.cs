using NLua;
using System.Text;
using System.Text.Json;

namespace LuaTableToJson
{
    public static class Parser
    {
        public static LuaTable LoadLuaTableFromFile(string path, bool isReturnWrapped)
        {
            Lua lua = new Lua();

            // read in text as string; UTF8 will prevent errors when using data with special characters
            var fileText = File.ReadAllText(path, Encoding.UTF8);


            var luaTableText = fileText;
            if (!isReturnWrapped)
            {
                luaTableText = "return {";
                luaTableText += fileText;
                luaTableText += "}";
            }


            // convert the raw text to LuaTable using NLua
            var table = lua.DoString(luaTableText)[0] as LuaTable;

            return table != null ? table : null;
        }

        // Recursive function to process the LuaTable into a c# dictionary
        public static Dictionary<string, object> ConvertLuaTableToDictionary(LuaTable table)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var key in table.Keys)
            {
                var keyString = key.ToString();
                var value = table[key];

                // ensure keystring is not null
                if (keyString != null)
                {
                    // Check if the value is a nested LuaTable
                    if (value is LuaTable nestedTable)
                    {
                        // Recursively process the nested table
                        dictionary[keyString] = ConvertLuaTableToDictionary(nestedTable);
                    }
                    else
                    {
                        // If it's a simple value, just add it
                        dictionary[keyString] = value;
                    }
                }
            }

            return dictionary;
        }

        public static string ConvertDictionaryToJson(Dictionary<string, object> dictionary)
        {
            return JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
