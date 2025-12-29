using UnityEngine;

public class Lock : Thing, IInteractable {
    public Crate crate;

    public bool Interact(GameObject handObject = null) {
        crate.ChangeLockMode();
        return false;
    }
}