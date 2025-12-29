using UnityEngine;

public interface IReceivable {
    void OnPlaced(GameObject placedObj, int index = 0);
    void OnRemoved(int index = 0);
}