using UnityEngine;

public class ReceivePlace : Building {
    [Header("ReceivePlace")]
    public bool isRound;

    public Building building;
    public int MyIndex = 0;

    GameObject curObj;
    IReceivable receivable;

    /**********/

    private void Start() {
        if (building is IReceivable)
            receivable = building as IReceivable;
        else
            Debug.Log("Здание " + building.gameObject.name + " не реализует интерфейс Receivable");
    }

    public void OnTriggerEnter(Collider other) {
        if (isRound) {
            if (!curObj && other.gameObject.GetComponent<Thing>() is Pickable) {
                curObj = other.gameObject;
                receivable.OnPlaced(other.gameObject, MyIndex);
            }
        } else if (!curObj && other.gameObject.GetComponent<Thing>() && other.gameObject.GetComponent<Thing>().thingName == "matterCube") {
            curObj = other.gameObject;
            receivable.OnPlaced(other.gameObject, MyIndex);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (curObj && other.gameObject == curObj) {
            receivable.OnRemoved(MyIndex);
            curObj = null;
        }
    }
}