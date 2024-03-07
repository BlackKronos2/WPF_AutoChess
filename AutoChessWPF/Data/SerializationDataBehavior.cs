using Newtonsoft.Json;
using System.IO;

namespace AutoChessWPF
{
    /// <summary> Сериализация и Десериализация данных </summary>
    public class SerializationDataBehavior
    {
        private const string Format = "";

        public static void Save<T>(T data, string fileName)
        {
            string serializeT = JsonConvert.SerializeObject(data);
            var file = File.Create(fileName);
            file.Close();

            File.WriteAllText(fileName, serializeT);
        }

        public static T Load<T>(string fileName)
        {
            string filePath = fileName + Format;

            string jsonText = File.ReadAllText(filePath);
            T data = JsonConvert.DeserializeObject<T>(jsonText);
            return data;
        }
    }
}
