// GoalZone.cs
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [Header("Solution")]
    public string correctLetter;

    [Header("Visuals")]
    public Color defaultColor = new Color(1f, 0f, 0f, 0.4f); // Semi-transparent red
    public Color correctColor = new Color(0f, 1f, 0f, 0.4f); // Semi-transparent green

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultColor;
    }

    // Public method for the GameManager to check this zone's status
    public bool IsCorrect()
    {
        // Check for a collider directly on top of this zone
        Collider2D hit = Physics2D.OverlapPoint(transform.position);

        if (hit != null && hit.CompareTag("Pushable"))
        {
            Letter letterOnTop = hit.GetComponent<Letter>();

            // The three win conditions for a single zone:
            bool isCorrectLetter = letterOnTop.letter.Equals(correctLetter.ToUpper());
            bool hasZeroMoves = letterOnTop.movesRemaining == 0;

            // All conditions must be true
            return isCorrectLetter && hasZeroMoves;
        }

        // If no pushable block is on top, it's not correct
        return false;
    }

    // This is called every frame to give instant visual feedback
    void Update()
    {
        if (IsCorrect())
        {
            spriteRenderer.color = correctColor;
        }
        else
        {
            spriteRenderer.color = defaultColor;
        }
    }
}
