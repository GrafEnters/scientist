using UnityEngine;

public class InterfaceNavigation : MonoBehaviour {
    public GameObject MenuPanel;
    bool CouldPlayerMove;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            HideShowMenuPanel();
    }

    void HideShowMenuPanel() {
        if (MenuPanel.activeSelf) {
            InputManager.CanPlayerMove = CouldPlayerMove;
            MenuPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            MenuPanel.SetActive(true);
            CouldPlayerMove = InputManager.CanPlayerMove;
            InputManager.CanPlayerMove = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ExitGame() {
        Application.Quit();
    }
}