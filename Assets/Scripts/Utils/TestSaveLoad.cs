using UnityEngine;

public class TestSaveLoad : MonoBehaviour {
    Game gameSession;
    public GameObject Player;
    public GameObject PlayerCamera;

    public KeyCode saveKey = KeyCode.K, loadKey = KeyCode.L;

    string datapath;

    private void Start() {
        datapath = Application.dataPath + "/Saves/TestData.xml";
    }

    void Update() {
        if (Input.GetKeyDown(saveKey))
            Save();
        if (Input.GetKeyDown(loadKey))
            Load();
    }

    void CreateSave() {
        gameSession = new Game();
        gameSession.player = new Player();
        gameSession.player.playerPos = Player.transform.position;
        gameSession.player.playerRot = Player.transform.rotation;
        gameSession.player.camLookUtil = PlayerCamera.GetComponent<MouseLook>().GetRotation();
        gameSession.player.camLook = PlayerCamera.transform.rotation;
    }

    public void ApplySave() {
        Player.GetComponent<CharacterController>().enabled = false;
        PlayerCamera.GetComponent<MouseLook>().enabled = false;

        PlayerCamera.GetComponent<MouseLook>().SetRotation(gameSession.player.camLookUtil);
        Player.transform.SetPositionAndRotation(gameSession.player.playerPos, gameSession.player.playerRot);
        PlayerCamera.transform.rotation = gameSession.player.camLook;

        Player.GetComponent<CharacterController>().enabled = true;
        PlayerCamera.GetComponent<MouseLook>().enabled = true;
    }

    public void Save() {
        CreateSave();
        Serializer.SaveXml(gameSession, datapath);
    }

    public void Load() {
        gameSession = null;
        gameSession = Serializer.DeXml(datapath);
        ApplySave();
    }
}