using UnityEngine;

public class Pickable : Thing {
    LayerMask magnetMask;
    public float magnetPower = 30;
    protected Rigidbody rb;

    public override void Init(int id, Matter main, Matter side) {
        base.Init(id, main, side);
        if (gameObject.transform.tag == "Untagged")
            gameObject.transform.tag = "Pickable";

        magnetPower = 5;
        magnetMask = LayerMask.GetMask("Default", "Structure");
    }

    protected virtual void Start() {
        rb = GetComponent<Rigidbody>();
        if (mainM.BouncingScale > 0) {
            foreach (Collider coll in GetComponents<Collider>()) {
                coll.material.bounciness = 0.95f;
            }
        }

        if (thingName == "crate")
            foreach (Collider coll in GetComponents<Collider>()) {
                coll.material.bounciness = 0;
            }
    }

    public virtual bool Use() {
        return false;
    }

    public virtual bool SideUse() {
        return false;
    }

    private void FixedUpdate() {
        if (mainM.isMagnet && rb) {
            Collider[] colls = Physics.OverlapSphere(transform.position, 5);
            if (colls.Length > 0) {
                int cnt = 0;
                for (int i = 0; i < colls.Length; i++) {
                    if (cnt >= 3)
                        break;
                    if (colls[i].gameObject != gameObject && colls[i].gameObject.GetComponent<Thing>() &&
                        colls[i].gameObject.GetComponent<Thing>().mainM.isMagnet) {
                        cnt++;

                        Vector3 target = colls[i].transform.position;
                        Vector3 direction = target - transform.position;
                        rb.AddForce((direction).normalized * magnetPower / (direction.magnitude + 0.001f), ForceMode.Acceleration);
                    }
                }
            }
        }
    }
}