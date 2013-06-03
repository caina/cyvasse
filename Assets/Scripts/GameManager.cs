using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
	public bool isFirstPlayer = true;
	private Chat chat;
	public Board gameBoard;
	
    void OnJoinedRoom()
    {
        StartGame();
		chat =  this.GetComponent<Chat>();
		SendChatMessage("Entrou na sala");
    }
	
	void SendChatMessage(string message){
		chat.chatInput = message;
		chat.SendChat(PhotonTargets.All);
	}
    
    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        Application.LoadLevel(Application.loadedLevel);

    }
	
    void StartGame()
    {
        
		Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    
		
		photonView.RPC("PlayerConnected", PhotonTargets.All);
		
        //prepare instantiation data for the viking: Randomly diable the axe and/or shield
        /*bool[] enabledRenderers = new bool[2];
        enabledRenderers[0] = Random.Range(0,2)==0;//Axe
        enabledRenderers[1] = Random.Range(0, 2) == 0; ;//Shield
        
        object[] objs = new object[1]; // Put our bool data in an object array, to send
        objs[0] = enabledRenderers;

        // Spawn our local player
        PhotonNetwork.Instantiate(this.playerPrefabName, transform.position, Quaternion.identity, 0, objs);*/
		
    }
	
	[RPC]
	void PlayerConnected(){
		if(isFirstPlayer){
			isFirstPlayer = false;	
		}else{
		/*
			Camera.main.transform.position = new Vector3(0f,6.84f,-5.7f);
			Vector3 rot = transform.rotation.eulerAngles;
			Camera.main.transform.rotation = Quaternion.Euler(53.65f,rot.x,rot.y);	
			
			*/
			RequestshuffleParts();
		
		}
	}
	
	[RPC]
	void RequestshuffleParts(){
		gameBoard.ShuffleParts();
		SendChatMessage("embaralharemos as pecas! preparem-se!");
		
	}



    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave& QUIT"))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
	
	void OnPhotonPlayerDisconnected(PhotonPlayer player){
    	SendChatMessage("Player Desconectado: " + player);
	}

    void OnDisconnectedFromPhoton()
    {
        SendChatMessage("Disconectado do servidor");
		Debug.LogWarning("OnDisconnectedFromPhoton");
    }
    void OnFailedToConnectToPhoton()
    {
        Debug.LogWarning("OnFailedToConnectToPhoton");
    }
  
}
