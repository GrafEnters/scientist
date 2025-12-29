using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class ConfigsProvider : MonoBehaviour {
    public static ConfigsProvider Instance;

    [field: SerializeField]
    public MainGameConfig MainGameConfig { get; set; }

    private void Awake() {
        Instance = this;
    }
}