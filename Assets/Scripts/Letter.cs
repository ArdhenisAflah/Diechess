// Letter.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Letter : MonoBehaviour, IPointerDownHandler
{
    // --- Visuals & Data ---
    public string letter = "W";
    public int movesRemaining = 5;

    // --- Component Links ---
    [Header("Link these in the Inspector")]
    public TextMeshPro letterText;
    public TextMeshPro moveCountText;
    public SpriteRenderer background;

    [Header("Colors")]
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color noMovesColor = Color.grey;

    void Start()
    {
        UpdateVisuals();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (movesRemaining > 0)
        {
            GameManager.instance.SelectLetter(this);
        }
    }

    public void DecreaseMove()
    {
        if (movesRemaining > 0)
        {
            movesRemaining--;
            UpdateVisuals();

            if (movesRemaining <= 0)
            {
                GameManager.instance.DeselectIfMatches(this);
            }
        }
    }

    public void Select()
    {
        if (movesRemaining > 0)
        {
            background.color = selectedColor;
        }
    }

    public void Deselect()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        letter = letter.ToUpper();
        letterText.text = letter;
        moveCountText.text = movesRemaining.ToString();

        if (movesRemaining > 0)
        {
            background.color = defaultColor;
        }
        else
        {
            background.color = noMovesColor;
            moveCountText.text = "";
        }
    }
}
