using UnityEngine;

public class InvisibleTrigger : Thing, IInteractable {
    public Building building;

    public bool Interact(GameObject handObject = null) {
        building.Activate();
        return false;
    }
}