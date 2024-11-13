using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Chess
{
    public class ChessClient : Client
    {
        public override GameType GameType => GameType.Chess;

        protected override void ProcessGameServerNotification(JsonArray arguments)
        {

        }

        protected override void ProcessGameServerRequest(JsonArray arguments)
        {

        }

        protected override void ProcessGameServerResponse(JsonArray arguments)
        {

        }
    }
}
