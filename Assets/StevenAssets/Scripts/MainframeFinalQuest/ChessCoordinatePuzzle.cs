using TMPro;
using UnityEngine;

public class ChessCoordinatePuzzle : MonoBehaviour
{
    [Header("Answer")]
    [Range(0, 7)] public int answerFile = 1; // B
    [Range(1, 8)] public int answerRank = 3; // 3

    [Header("UI")]
    public TMP_Text coordinateText;
    public TMP_Text statusText;

    private int currentFile;
    private int currentRank = 1;
    private bool solved;

    public bool IsSolved => solved;

    private void Start()
    {
        Refresh();
    }

    public void NextFile()
    {
        if (solved) return;
        currentFile = (currentFile + 1) % 8;
        SetStatus("SELECT A COORDINATE, THEN SUBMIT");
        RefreshCoordinate();
    }

    public void NextRank()
    {
        if (solved) return;
        currentRank++;
        if (currentRank > 8) currentRank = 1;
        SetStatus("SELECT A COORDINATE, THEN SUBMIT");
        RefreshCoordinate();
    }

    public void Submit()
    {
        if (solved) return;

        if (currentFile == answerFile && currentRank == answerRank)
        {
            solved = true;
            GoldenKeyQuestState.ChessSolved = true;
            SetStatus($"{CoordinateName(currentFile, currentRank)} ACCEPTED // COMMON NEIGHBOR VERIFIED");
        }
        else
        {
            SetStatus($"{CoordinateName(currentFile, currentRank)} REJECTED // CHECK BOTH KNIGHT NODES");
        }
    }

    public void ResetSelection()
    {
        if (solved) return;
        currentFile = 0;
        currentRank = 1;
        SetStatus("SELECT A COORDINATE, THEN SUBMIT");
        RefreshCoordinate();
    }

    private void Refresh()
    {
        RefreshCoordinate();
        if (GoldenKeyQuestState.ChessSolved)
        {
            solved = true;
            SetStatus("B3 ACCEPTED // COMMON NEIGHBOR VERIFIED");
        }
        else
        {
            SetStatus("SELECT A COORDINATE, THEN SUBMIT");
        }
    }

    private void RefreshCoordinate()
    {
        if (coordinateText != null)
            coordinateText.text = CoordinateName(currentFile, currentRank);
    }

    private void SetStatus(string value)
    {
        if (statusText != null)
            statusText.text = value;
    }

    private static string CoordinateName(int file, int rank)
    {
        return $"{(char)('A' + file)}{rank}";
    }
}
