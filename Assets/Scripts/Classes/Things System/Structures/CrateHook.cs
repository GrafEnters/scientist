using UnityEngine;

public class CrateHook : Building, IInteractable {
    public GameObject insideObject;
    public Transform hookPos;

    public GameObject GetObject() {
        GameObject res = null;

        if (insideObject) {
            if (insideObject.transform.parent == transform)
                res = insideObject;

            insideObject = null;
        }

        return res;
    }

    public bool Interact(GameObject handObj = null) {
        if (insideObject && insideObject.transform.parent == transform)
            return false;

        if (handObj && handObj.GetComponent<Tool>()) {
            insideObject = handObj;
            insideObject.SetActive(true);
            handObj.transform.SetPositionAndRotation(hookPos.position, Quaternion.identity);
            handObj.transform.SetParent(transform);
            handObj.layer = 0;
            handObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            handObj.GetComponent<Rigidbody>().isKinematic = true;
            handObj.GetComponent<Rigidbody>().detectCollisions = true;
            return true;
        } else
            Debug.Log("Крючок занят или нет объекта в руке или он не подходит");

        return false;
    }
}