using System.Collections;
using UnityEngine;

public class Hold : MonoBehaviour {
    public static float maxOperateDistance = 10;

    public KeyCode pickDropKey, useKey, sideUseKey, inventoryKey;
    public float useAnim, animSpeed, animDistance, animSmooth; // параметры анимации удара
    public float smoothRot;

    public LayerMask ignoreLookingLayers;
    public CursorScript cursorScript;
    GameObject inHandObject; // РЕФЕРЕНС на объект в инвентаре в скрипте инвентаря, НЕ САМОСТОЯТЕЛЬНЫЙ ОБЪЕКТ
    Pickable inHandPickable; // РЕФЕРЕНС на объект в инвентаре в скрипте инвентаря, НЕ САМОСТОЯТЕЛЬНЫЙ ОБЪЕКТ
    Rigidbody inHandRB; // Его rigidbody
    MouseDrag mouseDrag; //Скрипт для переноса объектов
    UseCrate useCrate; //Скрипт для переноса объектов
    GameObject hidedObject;
    Health HealthScript;

    private void Start() {
        maxOperateDistance = 10;
        mouseDrag = GetComponent<MouseDrag>();
        useCrate = GetComponent<UseCrate>();
        HealthScript = GetComponent<Health>();
    }

    void Update() {
        // Находим актуальный объект для переноса
        GameObject objectLookingAt = WhatIAmLookingOn();

        if (objectLookingAt) {
            if (Input.GetKeyDown(useKey) && InputManager.CanPlayerMove)
                if (TryToStartCarry(objectLookingAt))
                    Hide();

            if (Input.GetKeyDown(sideUseKey) && InputManager.CanPlayerMove) {
                Interact(objectLookingAt);
                UnHide();
            }
        }

        if (Input.GetKeyDown(pickDropKey) && InputManager.CanPlayerMove)
            if (objectLookingAt && objectLookingAt.GetComponent<Pickable>()) {
                mouseDrag.Drop();
                InitInHand(objectLookingAt);
            } else
                FromHandToCarried();

        if (Input.GetKeyDown(inventoryKey) && InputManager.CanPlayerMove) {
            if (mouseDrag.InitDragged(useCrate.UnhideCrate()))
                Hide();
        }
    }

    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Alpha1) && InputManager.CanPlayerMove) {
            GameObject tmp = useCrate.SwitchItems(inHandObject);
            if (tmp != inHandObject)
                InitInHand(tmp);
        }

        if (inHandObject) {
            CarryInHand();

            if (Input.GetKeyDown(useKey) && InputManager.CanPlayerMove)
                Use();

            if (Input.GetKeyDown(sideUseKey) && InputManager.CanPlayerMove)
                SideUse();
        }

        if (hidedObject && Input.GetKeyUp(useKey) && InputManager.CanPlayerMove)
            UnHide();
    }

    public void InitInHand(GameObject obj) {
        if (!(obj && obj.GetComponent<Crate>()))
            Drop();

        if (obj)
            if (obj.GetComponent<Crate>()) {
                if (!obj.GetComponent<Crate>().isLocked)
                    useCrate.TakeCrate(obj);
            } else {
                inHandObject = obj;
                inHandPickable = inHandObject.GetComponent<Pickable>();
                inHandRB = inHandObject.GetComponent<Rigidbody>();

                inHandRB.constraints = RigidbodyConstraints.None;
                inHandRB.detectCollisions = false;
                inHandRB.useGravity = false;
                inHandRB.isKinematic = false;

                inHandObject.SetActive(true);
                inHandObject.transform.SetParent(transform);
                inHandObject.layer = 9; // Inhand layer

                ReturnCollision(inHandRB);
            }
    }

    void Interact(GameObject cargo) {
        if (cargo.GetComponent<Thing>() is IInteractable) {
            if (cargo.GetComponent<IInteractable>().Interact(inHandObject))
                Drop(false);
        }
    }

    public void Hide() {
        if (inHandObject) {
            hidedObject = inHandObject;
            hidedObject.SetActive(false); // Потом анимация
            inHandObject = null;
            inHandRB = null;
            inHandPickable = null;
        }
    }

    void UnHide() {
        if (hidedObject) {
            inHandObject = hidedObject;
            inHandRB = hidedObject.GetComponent<Rigidbody>();
            inHandPickable = hidedObject.GetComponent<Pickable>();
            inHandObject.SetActive(true); // Потом анимация
            hidedObject = null;
        }
    }

    void Use() {
        if (inHandPickable.Use()) {
            useAnim = 1.5f;
            animSpeed = 5f;
        }
    }

    void SideUse() {
        if (inHandPickable is IEdible) {
            HealthScript.Eat(inHandPickable as IEdible);
            Destroy(inHandPickable.gameObject);
        }

        if (inHandPickable.SideUse()) {
            useAnim = 2f;
            animSpeed = 3f;
        }
    }

    void FromHandToCarried() {
        mouseDrag.InitDragged(Drop());
    }

    GameObject Drop(bool isNullifyParent = true) {
        UnHide();
        if (inHandObject && inHandRB && inHandPickable) {
            inHandRB.detectCollisions = true;
            inHandRB.useGravity = true;
            inHandRB = null;
            if (isNullifyParent) {
                inHandObject.transform.SetParent(null);
                inHandPickable.transform.SetParent(null);
            }

            inHandPickable = null;
        }

        GameObject res = inHandObject;

        inHandObject = null;
        return res;
    }

    void CarryInHand() {
        useAnim = Mathf.Abs(useAnim - animSpeed * Time.deltaTime);
        inHandRB.linearVelocity =
            ((Camera.main.transform.position + Camera.main.transform.right * 3f / 4 - Camera.main.transform.up / 4 +
                 Camera.main.transform.forward * animDistance / 4f + Camera.main.transform.forward * useAnim -
                 inHandObject.transform.position) *
             animSmooth);
        Quaternion target = Quaternion.Euler(0, 90, -20 + Camera.main.transform.rotation.eulerAngles.x + useAnim * 90);
        inHandObject.transform.localRotation = Quaternion.Slerp(inHandObject.transform.localRotation, target, Time.deltaTime * smoothRot);
    }

    GameObject WhatIAmLookingOn() {
        // Настройка луча из камеры игрока
        int x = Screen.width / 2;
        int y = Screen.height / 2;
        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y));

        GameObject obj;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance: maxOperateDistance, ~ignoreLookingLayers)) {
            obj = hit.collider.GetComponent<Transform>().gameObject;
            if (obj.GetComponent<Crate>() && obj.GetComponent<Crate>().isLocked) {
                cursorScript.ChangeIcon(Center_Icons.Dot);
                return null;
            } else {
                if (obj.GetComponent<Pickable>())
                    cursorScript.ChangeIcon(Center_Icons.Hand);
                else if (obj.GetComponent<Thing>() is IInteractable)
                    cursorScript.ChangeIcon(Center_Icons.Gear);
                else
                    cursorScript.ChangeIcon(Center_Icons.Dot);
            }
        } else {
            obj = null;
            cursorScript.ChangeIcon(Center_Icons.Dot);
        }

        return obj;
    }

    bool TryToStartCarry(GameObject cargoObj = null) {
        if (cargoObj && cargoObj.GetComponent<Pickable>()) {
            if (!cargoObj.GetComponent<Crate>())
                cargoObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            else if (cargoObj.GetComponent<Crate>().isLocked)
                return false;

            mouseDrag.InitDragged(cargoObj);
            return true;
        }

        return false;
    }

    // Возвращает объекту возможность сталкиваться после того, как мы взяли его в руку
    IEnumerator ReturnCollision(Rigidbody rb) {
        yield return new WaitForSeconds(0.1f);
        rb.detectCollisions = true;
    }
}