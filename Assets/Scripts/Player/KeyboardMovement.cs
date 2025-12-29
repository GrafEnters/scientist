using UnityEngine;

public class KeyboardMovement : MonoBehaviour {
    public CharacterController Controller;

    public Transform GroundCheckTransform; // Проверка земли для прыжка и гравитации
    public float groundDistance = 0.4f;
    public LayerMask groundMask; // Какие слои считать землей

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f; // Высота прыжка в юнитах

    Vector3 velocity; // Скорость с учетом гравитации
    bool isGrounded; // Стоим ли мы на земле

    void Update() {
        if (transform.position.y <= -50)
            velocity.y = Mathf.Sqrt(jumpHeight * 20 * -2f * gravity);

        // ХОДЬБА

        // Считываем кнопки и собираем направление движения
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Перемещаем игрока в этом направлении
        if (Controller.enabled)
            Controller.Move(move * speed * Time.deltaTime);

        // ПРЫЖОК

        // Вокруг метки на нижней стороне игрока делаем сферу проверки, приблизились мы к землеслоям на нужное расстояние
        isGrounded = Physics.CheckSphere(GroundCheckTransform.position, groundDistance, groundMask);

        // Если мы на земле, сбрасываем скорость от гравитации, чтобы прыгать одинакого
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Вычисляем вертикальную скорость в зависимости от высоты прыжка и гравитации
        if (Input.GetButtonDown("Jump") && InputManager.CanPlayerMove && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Учитываем ускорение гравитации
        velocity.y += gravity * Time.deltaTime;

        // Перемещаем игрока 
        if (Controller.enabled)
            Controller.Move(velocity * Time.deltaTime);
    }
}