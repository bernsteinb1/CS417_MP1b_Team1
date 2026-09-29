using UnityEngine;

public static class GoldenKeyQuestState
{
    public static bool CardRetrieved;
    public static bool CardScanned;
    public static bool ChessSolved;

    public static bool FinalGateReady => CardScanned && ChessSolved;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        CardRetrieved = false;
        CardScanned = false;
        ChessSolved = false;
    }
}

