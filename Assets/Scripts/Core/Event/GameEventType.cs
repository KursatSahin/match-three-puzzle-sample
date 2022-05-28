namespace Core.Event
{
    public enum GameEventType : ushort
    {
        GameStart,
        GameEnd,

        UpdateScore,
        
        BlockInputHandler,
        UnblockInputHandler,
    }
}