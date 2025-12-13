using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class Card : MonoBehaviour
{
    public Image iconImage;

    
    public Sprite pokemonImg;

    public GameObject backFace;
    public GameObject frontFace;

    public bool isSelected;

    public CardController controller;
    public Button cardButton;

    public int CardIndex;

    public void OnCardClick()
    {
        controller.SetSelected(this);
    }

    public void SetPokemon(Sprite sp)
    {
        pokemonImg = sp;
        iconImage.sprite = pokemonImg;
        cardButton = GetComponent<Button>();
    }

    public void Show()
    {
        Tween.Rotation(transform,
            new Vector3(0f, 180f, 0f),
            0.2f);
        Tween.Delay(0.1f,
            () =>
            {
                frontFace.SetActive(true);
                backFace.SetActive(false);
            });
       
        isSelected = true;
    }
    public void Hide()
    {
        Tween.Rotation(transform,
           new Vector3(0f, 0f, 0f),
           0.2f);
        Tween.Delay(0.1f,
            () =>
            {
                frontFace.SetActive(false);
                backFace.SetActive(true);
                isSelected = false;
            });

       
    }

}
