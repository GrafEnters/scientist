using UnityEngine;

public class FireLab : Building, IInteractable {
    public Transform In;
    public Vector3 halfExtense;

    /**********/

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(In.position, halfExtense);
    }

    /**********/

    public bool Interact(GameObject handObject = null) {
        Collider[] colls = Physics.OverlapBox(In.position, halfExtense / 2, Quaternion.identity, ~IgnoreMask);
        GameObject toBurn = null;
        if (colls.Length > 0)
            toBurn = colls[0].gameObject;

        if (toBurn) {
            if (toBurn.transform.tag == "Pickable") {
                Thing thing = toBurn.GetComponent<Thing>();
                thing.Burn();
            }
        }

        return false;
    }
}