using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public Transform PlayerTransform; // Референс на объект игрока

	public float mouseSensitivity = 100f; // Чувствительность мыши, больше -> больше вращение

	float vertCamRotation = 0f; // Абсолютный градус вертикального повторота камеры

	/**********/

	void Start()
	{
		// Мышка не елозит по экрану, а двигает камеру
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		float mouseX = 0, mouseY = 0;

		// Сдвиг коррдинат, определемый через позицию мышки
		if (InputManager.CanPlayerMove)
		{
			 mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
			 mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
		}

		// Определяем вертикальный угол поворота камеры
		vertCamRotation -= mouseY;
		vertCamRotation = Mathf.Clamp(vertCamRotation, -90f, 90f); // Ограничиваем, чтобы не смотреть за голову

		// Поворачиваем камеру вверх/вниз на нужное кол-во градусов
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(vertCamRotation, 0f, 0f), 0.5f); 

		// Горизонтально вращаем всего игрока
		PlayerTransform.rotation = Quaternion.Lerp(PlayerTransform.rotation, Quaternion.Euler(0f, transform.rotation.eulerAngles.y + mouseX, 0f), 0.5f);
	}

	/**********/

	// Вспомогательные скрипты, чтобы получать вращение для сейв/лоад
	public void SetRotation(float value)
	{
		vertCamRotation = value;
	}

	public float GetRotation()
	{
		return vertCamRotation;
	}
}
