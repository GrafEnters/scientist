using UnityEngine;

public class TimeManager : MonoBehaviour {
    static float WorldTime;
    static float DayTime;

    static int DayLength;
    public int DayLengthInSeconds;

    private void Awake() {
        WorldTime = 0;
        DayLength = DayLengthInSeconds;
    }

    private void Update() {
        WorldTime += Time.deltaTime;
        DayTime = WorldTime % (DayLength);
    }

    public static int GetDayLength() {
        return DayLength;
    }

    public static float GetDayTime() {
        return DayTime;
    }

    public static float GetWorldTime() {
        return WorldTime;
    }
}