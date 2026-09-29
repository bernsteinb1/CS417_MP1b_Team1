using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessKnightPuzzle : MonoBehaviour
{
    public TMP_Text statusText;
    public List<Renderer> squares = new List<Renderer>();

    private int currentX;
    private int currentY;
    private int moves;
    private bool solved;

    private static readonly Color Light = new Color(0.82f, 0.82f, 0.78f);
    private static readonly Color Dark = new Color(0.12f, 0.12f, 0.14f);
    private static readonly Color Path = new Color(0.15f, 0.65f, 1f);
    private static readonly Color Goal = new Color(1f, 0.72f, 0.05f);

    public void Initialize()
    {
        ResetPuzzle();
    }

    public void SelectSquare(int x, int y)
    {
        if (solved) return;

        int dx = Mathf.Abs(x - currentX);
        int dy = Mathf.Abs(y - currentY);
        bool legalKnightMove = (dx == 2 && dy == 1) || (dx == 1 && dy == 2);

        if (!legalKnightMove)
        {
            ResetPuzzle("ILLEGAL MOVE — ROUTE RESET");
            return;
        }

        currentX = x;
        currentY = y;
        moves++;
        Paint(x, y, Path);

        if (x == 7 && y == 7)
        {
            if (moves == 6)
            {
                solved = true;
                GoldenKeyQuestState.ChessSolved = true;
                Paint(7, 7, new Color(0.2f, 1f, 0.3f));
                SetStatus("SHORTEST PATH VERIFIED — CHESS AUTH ACCEPTED");
            }
            else
            {
                ResetPuzzle("TARGET REACHED, BUT NOT SHORTEST — RESET");
            }
            return;
        }

        if (moves >= 6)
        {
            ResetPuzzle("6 MOVES USED WITHOUT REACHING H8 — RESET");
            return;
        }

        SetStatus($"MOVE {moves}/6 — CURRENT {SquareName(currentX, currentY)}");
    }

    public void ResetPuzzle(string message = null)
    {
        solved = false;
        if (!GoldenKeyQuestState.ChessSolved)
        {
            currentX = 0;
            currentY = 0;
            moves = 0;
            RepaintBoard();
            Paint(0, 0, Path);
            Paint(7, 7, Goal);
            SetStatus(message ?? "START A1 → TARGET H8 — EXACTLY 6 KNIGHT MOVES");
        }
    }

    private void RepaintBoard()
    {
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
            Paint(x, y, ((x + y) & 1) == 0 ? Light : Dark);
    }

    private void Paint(int x, int y, Color color)
    {
        int index = y * 8 + x;
        if (index >= 0 && index < squares.Count && squares[index] != null)
            squares[index].material.color = color;
    }

    private void SetStatus(string s)
    {
        if (statusText != null) statusText.text = s;
    }

    private static string SquareName(int x, int y) => $"{(char)('A' + x)}{y + 1}";
}
