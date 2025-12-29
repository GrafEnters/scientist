using System.Collections;
using UnityEngine;

public class Seed : Pickable {
    public GameObject parentStructure;

    public float growTime = 15;

    float growRemainTime;

    protected override void Start() {
        base.Start();
        growRemainTime = growTime;
    }

    private void OnEnable() {
        StartCoroutine(Growing());
        //	Debug.Log("start Growing");
    }

    IEnumerator Growing() {
        while (growRemainTime >= 0) {
            yield return new WaitForSeconds(1);

            if (GetComponent<Rigidbody>().linearVelocity.sqrMagnitude > 0.01f)
                growRemainTime = growTime;

            LayerMask mask = LayerMask.GetMask("Ground");
            if (Physics.CheckSphere(transform.position, 0.3f, mask))
                growRemainTime -= 1;
            else
                growRemainTime = growTime;
        }

        PlantNewStructure();
    }

    void PlantNewStructure() {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, 1f)) {
            GameObject ground = hit.collider.GetComponent<Transform>().gameObject;

            // Если структура - разрушаем
            if (ground.GetComponent<Chunk>()) {
                GameObject structure = Instantiate(parentStructure, hit.point, Quaternion.identity);
                structure.SetActive(true);
                Destroy(gameObject);
                return;
            }
        }
    }
}