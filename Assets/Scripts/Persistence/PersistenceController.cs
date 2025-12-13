using System.Collections.Generic;
using UnityEngine;

public class PersistenceController : MonoBehaviour
{
    private const string SAVE_KEY = "CARD_MATCH_SAVE";

    public PatternGridGenerator gridGenerator;
    public CardController controller;

    
    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    //Save
    public void SaveGame()
    {
        if (controller.gameOver)
            return;

        Debug.Log("Saving game...");
        string k = controller.GetCurrentGameKey();
        Debug.Log(k);
        int rows = 0;
        int columns = 0;
        switch (k)
        {
            case "2X2":
                rows = 2;
                columns = 2;
                break;
            case "2X4":
                rows = 2;
                columns = 4;
                break;
            case "4X4":
                rows = 4;
                columns = 4;
                break;
            case "4X5":
                rows = 4;
                columns = 5;
                break;
        }
        controller.ResetOpenCard();
        GameState state = new GameState
        {
            rows = rows,
            columns = columns,
            gameId = controller.id,
            turnCount = controller.turnCount,
            matchedCount = controller.matchedCount,
            isGameOver = controller.gameOver,
            cards = new List<CardState>()
        };

        Card[] cards = gridGenerator.grid.GetComponentsInChildren<Card>();

        foreach (Card card in cards)
        {
            CardState cs = new CardState
            {
                index = card.CardIndex,
                spriteName = card.pokemonImg.name,
                isMatched = card.iconImage.color == Color.gray,
                isRevealed = card.isSelected
                
            };

            state.cards.Add(cs);
        }

        string json = JsonUtility.ToJson(state);
        string key = SAVE_KEY + controller.GetCurrentGameKey();
        Debug.Log("Saving game with key: " + key);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();

        Debug.Log("Game Saved");
    }
    public bool isGamePending()
    {
        Debug.Log("Checking for save key: " + (SAVE_KEY + controller.GetCurrentGameKey())+ PlayerPrefs.HasKey(SAVE_KEY + controller.GetCurrentGameKey()));
        return PlayerPrefs.HasKey(SAVE_KEY + controller.GetCurrentGameKey());
    }
    // Load
    public bool LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY+controller.GetCurrentGameKey()))
            return false;

        Debug.Log("Loading game with key: " + (SAVE_KEY + controller.GetCurrentGameKey()));
        string json = PlayerPrefs.GetString(SAVE_KEY + controller.GetCurrentGameKey());
        GameState state = JsonUtility.FromJson<GameState>(json);
        string key = controller.GetCurrentGameKey();
       
        // Build grid
        Debug.Log("Restoring grid with rows: " + state.rows + ", columns: " + state.columns);
        gridGenerator.BuildPattern(state.rows, state.columns);
        controller.StartWithID(state.gameId);

        controller.turnCount = state.turnCount;
        controller.matchedCount = state.matchedCount;

        controller.turnText.text = "Turns:\n" + state.turnCount;
        controller.scoreText.text = "Score:\n" + state.matchedCount;

        // Restore cards
        Card[] cards = gridGenerator.grid.GetComponentsInChildren<Card>();

        foreach (Card card in cards)
        {
            CardState cs = state.cards.Find(x => x.index == card.CardIndex);
            if (cs == null) continue;

            Sprite sprite = controller.allSprites.Find(s => s.name == cs.spriteName);
            card.SetPokemon(sprite);

            if (cs.isMatched)
            {
                card.iconImage.color = Color.gray;
                card.frontFace.SetActive(true);
                card.backFace.SetActive(false);
                card.isSelected = true;
            }
            else if (cs.isRevealed)
            {
                card.frontFace.SetActive(true);
                card.backFace.SetActive(false);
                card.isSelected = true;
                controller.ReveleadFromLoad(card);
            }
            else
            {
                card.frontFace.SetActive(false);
                card.backFace.SetActive(true);
                card.isSelected = false;
            }
        }
        /*int recalculatedMatchedPairs = 0;
        List<Card> revealedButUnmatched = new List<Card>();

        foreach (Card card in cards)
        {
            if (!card.frontFace.activeInHierarchy)
                continue;

            // Matched cards (gray)
            if (card.iconImage.color == Color.gray)
            {
                recalculatedMatchedPairs++;
            }
            else if (card.isSelected)
            {
                // Revealed but not matched (possible interrupted state)
                revealedButUnmatched.Add(card);
            }
        }

        // Each pair has 2 cards
        recalculatedMatchedPairs /= 2;

        // Force sync controller values
        controller.matchedCount = recalculatedMatchedPairs;
        controller.scoreText.text = "Score:\n" + controller.matchedCount;

        // Handle half-revealed edge case
        if (revealedButUnmatched.Count == 2)
        {
            // Safely assign for match resolution
            controller.ReveleadFromLoad(revealedButUnmatched[0]);
            controller.ReveleadFromLoad(revealedButUnmatched[1]);
        }

        // ================= FINAL WIN CHECK =================

        if (controller.matchedCount >= controller.totalPairs)
        {
            Debug.Log("Loaded game already completed — triggering win");
            controller.HandleWinFromLoad();
        }*/
        Debug.Log("Game Loaded");
        return true;
    }

    // Clear
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY + controller.GetCurrentGameKey());
    }
}
