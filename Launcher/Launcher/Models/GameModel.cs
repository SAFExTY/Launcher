namespace AuthServer.Models
{
    public class GetGameModel
    {
        public string GameId { get; set; }
    }

    public class UpdateGameModel : GetGameModel
    {
        public string Game { get; set; }
    }
}