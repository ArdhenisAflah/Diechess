// GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Objects")]
    public GameObject winScreen;

    [Header("Physics Settings")]
    public LayerMask blockingLayer; // This will filter what blocks movement

    private GoalZone[] allGoalZones;
    private Letter selectedLetter;
    private int movingObjectsCount = 0;
    private bool isLevelComplete = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        allGoalZones = FindObjectsOfType<GoalZone>();

        if (winScreen != null) winScreen.SetActive(false);
    }

    void Update()
    {
        if (selectedLetter != null && movingObjectsCount == 0 && !isLevelComplete)
        {
            HandleMovementInput();
        }
    }

    public void SelectLetter(Letter letterToSelect)
    {
        if (isLevelComplete) return;
        if (selectedLetter != null) selectedLetter.Deselect();
        selectedLetter = letterToSelect;
        selectedLetter.Select();
    }

    public void DeselectIfMatches(Letter letter)
    {
        if (selectedLetter == letter)
        {
            selectedLetter.Deselect();
            selectedLetter = null;
        }
    }

    void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2.right);
    }

    private void Move(Vector2 direction)
    {
        if (selectedLetter == null || movingObjectsCount > 0 || selectedLetter.movesRemaining <= 0)
        {
            return;
        }

        List<Transform> pushChain = new List<Transform>();
        Vector2 currentCheckPos = (Vector2)selectedLetter.transform.position + direction;

        while (true)
        {
            // Use the LayerMask to ignore Goal colliders
            Collider2D hit = Physics2D.OverlapPoint(currentCheckPos, blockingLayer);
            if (hit == null)
            {
                break;
            }

            if (hit.CompareTag("Pushable"))
            {
                pushChain.Add(hit.transform);
                currentCheckPos += direction;
            }
            else
            {
                return;
            }
        }

        StartCoroutine(MoveObject(selectedLetter.transform, (Vector2)selectedLetter.transform.position + direction));
        foreach (Transform block in pushChain)
        {
            StartCoroutine(MoveObject(block, (Vector2)block.position + direction));
        }
        selectedLetter.DecreaseMove();
    }

    private IEnumerator MoveObject(Transform objTransform, Vector2 endPosition)
    {
        movingObjectsCount++;
        float elapsedTime = 0;
        float moveTime = 0.15f;
        Vector2 startPosition = objTransform.position;

        while (elapsedTime < moveTime)
        {
            objTransform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objTransform.position = endPosition;
        movingObjectsCount--;

        if (movingObjectsCount == 0)
        {
            CheckForWin();
        }
    }

    void CheckForWin()
    {
        if (isLevelComplete) return;

        foreach (GoalZone zone in allGoalZones)
        {
            if (!zone.IsCorrect())
            {
                return;
            }
        }

        Debug.Log("LEVEL COMPLETE!");
        isLevelComplete = true;
        if (winScreen != null) winScreen.SetActive(true);
    }
}