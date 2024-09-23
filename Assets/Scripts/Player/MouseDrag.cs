using UnityEngine;

public class MouseDrag : MonoBehaviour
{
	// Когда игрок наводит мышку на предмет и удерживает лкм, мы можем его тащить
	// Когда он отпускает лкм, предмет падает
	// Когда игрок тащит объект и жмет пкм, он вращается

	public float distance, smooth, rotationDegreesPerSecond = 10;

	GameObject draggedObject;
	Rigidbody draggedRB;
	float curMass;
	Vector3 rotationVctr;
	Vector3 rotationStep;
	private void Update()
	{
		if (draggedObject)
		{
			Drag();

			if (Input.GetMouseButtonDown(1) && InputManager.CanPlayerMove)
				SwitchRotation();
			if (Input.GetMouseButton(1) && InputManager.CanPlayerMove)
				Rotate();
			if (Input.GetMouseButtonUp(0) && InputManager.CanPlayerMove)
				Drop();

		}
	}

	public bool InitDragged(GameObject obj)
	{
		if (obj)
		{
			draggedObject = obj;
			draggedObject.layer = 11; // Carry layer
			draggedObject.SetActive(true);
			rotationStep = Vector3.forward;
			draggedRB = draggedObject.GetComponent<Rigidbody>();
			curMass = draggedRB.mass;
			draggedRB.mass = 0.001f;
			rotationVctr = new Vector3(0, 0, 0);
			draggedObject.transform.SetParent(transform);
			draggedRB.useGravity = false;
			return true;
		}
		return false;
	}

	void Drag()
	{
		if(draggedObject.GetComponent<Pickable>().thingName == "crate")
			draggedRB.linearVelocity = ((Camera.main.transform.position + Camera.main.transform.forward * (distance+2) - draggedObject.transform.position) * smooth);
		else
			draggedRB.linearVelocity = ((Camera.main.transform.position + Camera.main.transform.forward * distance - draggedObject.transform.position) * smooth);
		
		draggedObject.transform.rotation = Quaternion.identity;
		Quaternion target = Quaternion.Euler(rotationVctr);
		draggedObject.transform.localRotation = target; //Quaternion.Slerp(draggedObject.transform.localRotation, target, Time.deltaTime * smoothRot);
	}

	public void Drop()
	{
		if (draggedObject)
		{
			draggedRB.useGravity = true;
			draggedRB.mass = curMass;
			draggedRB.mass = 1000;
			draggedRB.linearVelocity = Vector3.zero;
			draggedRB.angularVelocity = Vector3.zero;
			draggedObject.layer = 0;
			draggedObject.transform.SetParent(null);

			draggedRB = null;
			draggedObject = null;
			
		}
	}

	void SwitchRotation()
	{
		
		if (rotationStep == Vector3.up)
			rotationStep = Vector3.right;
		else if (rotationStep == Vector3.right)
			rotationStep = Vector3.forward;
		else if (rotationStep == Vector3.forward)
			rotationStep = Vector3.up;
	}
	void Rotate()
	{
		rotationVctr += rotationStep * Time.deltaTime * rotationDegreesPerSecond;
	}
}
