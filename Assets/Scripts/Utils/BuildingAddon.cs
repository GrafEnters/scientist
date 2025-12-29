using UnityEngine;

public class BuildingAddon : Building, IInteractable {
    public Building mainBuilding;

    public bool Interact(GameObject handObject = null) {
        mainBuilding.SideActivate();
        return false;
    }
}