using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Unity.VisualScripting;

public class CardController : MonoBehaviour
{
    public List<Sprite> allSprites;
    
    [SerializeField] Transform gridTransform;

    public List<Sprite> spritePairs;

    Card firstSelected;
    Card secondSelected;

    public int matchedCount;
    public int turnCount;


    public TMPro.TMP_Text turnText;
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text bestscoreText;

    private string BEST_TURNS_KEY = "BESTSCORE";
    private List<string> CurrentGame = new List<string> { "2X2", "2X4", "4X4", "4X5" };

    public int id;// game id
    int bestScore;


    public void StartWithID(int id)
    {
        this.id = id;
        LoadBestScore();

    }
    public void SetSelected(Card card)
    {
        if (!card.isSelected)
        {
            card.Show();
            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }
            if (secondSelected == null)
            {
                secondSelected = card;
                StartCoroutine(CheckMatch(firstSelected,secondSelected));
               
            }
        }
    }

    IEnumerator CheckMatch(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);
        turnCount++;
        turnText.text = "Turns: " +"\n"+ turnCount;
        if (firstSelected.pokemonImg == secondSelected.pokemonImg)
        {
            // Match found
            matchedCount++;
            scoreText.text = "Score: " + "\n" + matchedCount;
            if (matchedCount >= spritePairs.Count/2)
            {
                Debug.Log("You Win!");
                PrimeTween.Sequence.Create()
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                    .Chain(PrimeTween.Tween.Scale(gridTransform, Vector3.zero, 0.1f))
                    .OnComplete(() =>
                     {
                         SaveBestScore(turnCount);
                         GetComponent<PersistenceController>().ClearSave();
                         GetComponent<PatternGridGenerator>().Home();
                     });
              

            }
            firstSelected.iconImage.color = Color.gray;
            secondSelected.iconImage.color = Color.gray;
            firstSelected = null;
            secondSelected = null;

           // if (matchedCount)
        }
        else
        {
            // No match
            
            firstSelected.Hide();
            secondSelected.Hide();
            firstSelected = null;
            secondSelected = null;

        }
    }

    #region SaveScore

    void SaveBestScore(int currentTurns)
    {
        // If no score saved yet → save immediately
        if (!PlayerPrefs.HasKey(BEST_TURNS_KEY + CurrentGame[id]))
        {
            PlayerPrefs.SetInt(BEST_TURNS_KEY + CurrentGame[id], currentTurns);
            PlayerPrefs.Save();
            return;
        }

        int bestTurns = bestScore;

        // Lower turns = better score
        if (currentTurns < bestTurns)
        {
            PlayerPrefs.SetInt(BEST_TURNS_KEY + CurrentGame[id], currentTurns);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region LoadScore
    public void LoadBestScore()
    {
        if (PlayerPrefs.HasKey(BEST_TURNS_KEY + CurrentGame[id]))
        {
            int bestTurns = PlayerPrefs.GetInt(BEST_TURNS_KEY + CurrentGame[id]);
            bestScore = bestTurns;
            // Optional UI display if needed
            Debug.Log("BestScore: " + bestTurns);
            Debug.Log(CurrentGame[id]);
            Debug.Log(BEST_TURNS_KEY + CurrentGame[id]);
            bestscoreText.text = "Best Score: " + "\n" + bestTurns;
        }
        else
        {
            bestscoreText.gameObject.SetActive(false);
        }
    }

    #endregion
}
