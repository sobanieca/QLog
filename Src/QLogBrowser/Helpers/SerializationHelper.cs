using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using QLogBrowser.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace QLogBrowser.Helpers
{
    public class SerializationHelper
    {
        public static QLogBrowserSettings Deserialize(string base64encoded)
        {
            byte[] bytes = Convert.FromBase64String(base64encoded);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(var ms = new MemoryStream(bytes))
            {
                ms.Position = 0;
                return (QLogBrowserSettings)binaryFormatter.Deserialize(ms);
            }
        }

        public static string Serialize(object obj)
        {
            string result = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, obj);
                byte[] bytes = ms.ToArray();
                result = Convert.ToBase64String(bytes);
            }
            return result;
        }
    }
}
