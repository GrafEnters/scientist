using UnityEngine;

public class ShelfCell : Building, IInteractable {
    public GeologStation geologStation;
    public TextMesh nameField;
    public Vector3 halfExtends;
    public Transform insideField;

    public Matter matter;
    GameObject cube;

    public bool Interact(GameObject handObject = null) {
        Collider[] colls = Physics.OverlapBox(insideField.position, halfExtends / 2, Quaternion.identity, ~IgnoreMask);
        GameObject cube = null;
        if (colls.Length > 0)
            cube = colls[0].gameObject;
        else
            Debug.Log("Нет куба");

        if (cube && cube.GetComponent<MatterCube>()) {
            if (geologStation.AddMatter(cube.GetComponent<MatterCube>().mainM, this))
                PutObject(cube);
        }

        return false;
    }

    public void PutObject(GameObject cube) {
        this.cube = cube;
        cube.transform.SetParent(insideField);
        cube.transform.position = insideField.position;
        cube.transform.rotation = Quaternion.identity;
        cube.GetComponent<Rigidbody>().isKinematic = true;
        cube.GetComponent<MeshCollider>().enabled = false;
    }

    public void SetName(string name) {
        nameField.text = name;
    }

    public GameObject GetCube() {
        return cube;
    }

    public string GetName() {
        return nameField.text;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(insideField.position, halfExtends);
    }
}