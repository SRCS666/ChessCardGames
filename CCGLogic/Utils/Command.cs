using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;

namespace CCGLogic.Utils
{
    public enum CmdType
    {
        CTRequest,
        CTResponse,
        CTNotification
    }

    public enum CmdWhere
    {
        CWRoom,
        CWClient
    }

    public enum CmdOperation
    {
        COSignup,
        COSignupResult,
        COSetProperty,
        COAddPlayer,
        COToggleReady,
        COGameOperation
    }

    public class Command(CmdWhere source, CmdWhere destination, CmdType type,
        CmdOperation operation, JsonArray arguments = null)
    {
        public CmdWhere Source { get; set; } = source;
        public CmdWhere Destination { get; set; } = destination;
        public CmdType Type { get; set; } = type;
        public CmdOperation Operation { get; set; } = operation;
        public JsonArray Arguments { get; set; } = arguments ?? [];

        public static Command Parse(byte[] bytes)
        {
            try
            {
                JsonDocument document = JsonDocument.Parse(bytes);
                JsonArray jsonArray = JsonDocumentToJsonArray(document);
                if (jsonArray.Count < 5) { return null; }

                int sourceInt = jsonArray[0].GetValue<int>();
                int destinationInt = jsonArray[1].GetValue<int>();
                int typeInt = jsonArray[2].GetValue<int>();
                int operationInt = jsonArray[3].GetValue<int>();
                JsonArray jsonArray1 = StringToJsonArray(jsonArray[4].ToString());

                if (!Enum.IsDefined(typeof(CmdWhere), sourceInt)) { return null; }
                if (!Enum.IsDefined(typeof(CmdWhere), destinationInt)) { return null; }
                if (!Enum.IsDefined(typeof(CmdType), typeInt)) { return null; }
                if (!Enum.IsDefined(typeof(CmdOperation), operationInt)) { return null; }

                CmdWhere source = (CmdWhere)sourceInt;
                CmdWhere destination = (CmdWhere)destinationInt;
                CmdType type = (CmdType)typeInt;
                CmdOperation operation = (CmdOperation)operationInt;

                return new(source, destination, type, operation, jsonArray1);
            }
            catch { return null; }
        }

        public byte[] ToBytes()
        {
            int source = Convert.ToInt32(Source);
            int destination = Convert.ToInt32(Destination);
            int type = Convert.ToInt32(Type);
            int operation = Convert.ToInt32(Operation);
            JsonArray arguments = CloneJsonArray(Arguments);

            JsonArray jsonArray = [source, destination, type, operation, arguments];
            return JsonSerializer.SerializeToUtf8Bytes(jsonArray);
        }

        private static JsonArray JsonDocumentToJsonArray(JsonDocument document)
        {
            JsonArray jsonArray = [];

            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                jsonArray.Add(element);
            }

            return jsonArray;
        }

        public static JsonArray StringToJsonArray(string str)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                JsonDocument document = JsonDocument.Parse(bytes);
                return JsonDocumentToJsonArray(document);
            }
            catch { return null; }
        }

        public static string JsonArrayToString(JsonArray array) =>
            Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(array));

        public static JsonArray CloneJsonArray(JsonArray array) =>
            StringToJsonArray(JsonArrayToString(array));
    }
}
