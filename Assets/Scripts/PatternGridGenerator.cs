using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatternGridGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 4;
    public int columns = 5;

    [Header("References")]
    public GridLayoutGroup grid;
    public GameObject cardPrefab;
    public GameObject spacerPrefab; // EMPTY placeholder
    public CardController controller;


    private bool[,] layoutMask;

    public GameObject menuObj;
    public GameObject gameObj;


    void Build5x4Pattern()
    {
        layoutMask = new bool[,]
        {
            { true, true, true, true, true },
            { true, true, false, true, true },
            { true, true, false, true, true },
            { true, true, true, true, true }
        };
    }
    public void Home()
    {
       // menuObj.SetActive(true);
        //gameObj.SetActive(false);
        SceneManager.LoadScene(0);
    }
    void BuildFullGridPattern(int rows, int columns)
    {
        layoutMask = new bool[rows, columns];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                layoutMask[r, c] = true;
            }
        }
    }
    public void BuildPattern(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        menuObj.SetActive(false);
        gameObj.SetActive(true);

        if (rows == 4 && columns == 5)
        {
            // Special case: 5x4 pattern (reference image)
            Build5x4Pattern();
        }
        else
        {
            BuildFullGridPattern(this.rows, this.columns);
        }

        GenerateGrid(this.rows, this.columns);
        
    }


    public void GenerateGrid(int r, int c)
    {
        // Clear old grid 
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        // Configure Grid 
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = c;

        // Count valid cells
        List<Vector2Int> validSlots = new();

        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                if (layoutMask[row, col])
                    validSlots.Add(new Vector2Int(row, col));
            }
        }

        int totalCells = validSlots.Count;

        // Validation 
        if (totalCells % 2 != 0)
        {
            Debug.LogError("Pattern must have even number of cells!");
            return;
        }

        int pairsNeeded = totalCells / 2;

        if (controller.allSprites.Count < pairsNeeded)
        {
            Debug.LogError("Not enough sprites to generate pairs! " + pairsNeeded);
            return;
        }

        // --- Select sprites ---
        controller.spritePairs.Clear();
        List<Sprite> selectedSprites =
            new List<Sprite>(controller.allSprites);

        Shuffle(selectedSprites);

        for (int i = 0; i < pairsNeeded; i++)
        {
            controller.spritePairs.Add(selectedSprites[i]);
            controller.spritePairs.Add(selectedSprites[i]);
        }

        Shuffle(controller.spritePairs);

        // --- Instantiate grid (cards + spacers) ---
        int cardIndex = 0;

        for (int row = 0; row < r; row++)
        {
            for (int col = 0; col < c; col++)
            {
                if (layoutMask[row, col])
                {
                    GameObject card = Instantiate(cardPrefab, grid.transform);
                    card.name = $"Card_{cardIndex}";

                    Card view = card.GetComponent<Card>();
                    view.SetPokemon(controller.spritePairs[cardIndex]);
                    view.CardIndex = cardIndex;
                    view.controller = controller;

                    cardIndex++;
                    Debug.Log("Card instantiated");
                }
                else
                {
                    // spacer occupies grid slot
                    Instantiate(spacerPrefab, grid.transform);
                }
            }
        }
    }

    // Fisher-Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    public void Build2X2() {
        BuildPattern(2, 2);
        controller.StartWithID(0);
    }
    public void Build2X4()
    {
        BuildPattern(2, 4);
        controller.StartWithID(1);
    }
    public void Build4X4()
    {
        BuildPattern(4, 4);
        controller.StartWithID(2);
    }
    public void Build4X5()
    {
        BuildPattern(4, 5);
        controller.StartWithID(3);
    }
}
