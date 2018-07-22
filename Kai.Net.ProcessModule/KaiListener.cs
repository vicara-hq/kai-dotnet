using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kai.Net.ProcessModule
{
    public static class KaiListener
    {
        internal static void Listen()
        {
            while (Application.running)
            {
                var inputString = Console.ReadLine();
                JObject input;
                try
                {
                    input = JObject.Parse(inputString);
                }
                catch (JsonReaderException)
                {
                    // Ignore malformed JSON data
                    continue;
                }

                if (input[Constants.Success]?.ToObject<bool>() != true)
                {
                    
                }
            }
        }
    }
}