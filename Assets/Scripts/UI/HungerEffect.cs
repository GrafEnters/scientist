using UnityEngine;

public class HungerEffect : MonoBehaviour {
    public float baseSaturation = 10;
    public float minSaturation = -100;

    public float hunger = 1f;

    public void SetHungerStage(float percent) { }

    private void Update() {
        SetHungerStage(hunger);
    }
}