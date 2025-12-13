using System.Collections.Generic;

[System.Serializable]
public class CardState
{
    public int index;
    public string spriteName;
    public bool isMatched;
    public bool isRevealed;
}

[System.Serializable]
public class GameState
{
    public int rows;
    public int columns;
    public int gameId;

    public int turnCount;
    public int matchedCount;

    public List<CardState> cards;
}
