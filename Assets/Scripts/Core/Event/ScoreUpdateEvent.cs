namespace Core.Event
{
    public class ScoreUpdateEvent : IEvent
    {
        public int Score { get; set; }
        
        public ScoreUpdateEvent(int score)
        {
            Score = score;
        }
    }
}