using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
	public bool isFirstPlayer = true;
	private Chat chat;
	public Board gameBoard;
	public int playerRound = 0;
	public int playerTime;
	public bool isLocal = false;
	private PhotonView photonView;
	
	void Start(){
		photonView = (PhotonView) this.GetComponent<PhotonView>();	
	}
	
    void OnJoinedRoom()
    {
        StartGame();
		chat =  this.GetComponent<Chat>();
		SendChatMessage("Entrou na sala");
    }
	
	void SendChatMessage(string message){
		if(PhotonNetwork.connected){
			chat.chatInput = message;
			chat.SendChat(PhotonTargets.All);	
		}
		
	}
	
	/***
	 * Muda o turno atual.
	 * Comeca sempre com o player 1*/
	public void nextRound(){
		photonView.RPC("changeRound",PhotonTargets.All);
	}
	
	void changeRound(){
		playerRound = (playerRound==1?0:1);	
	}
	
	/***
	 * Compara a rodada atual com o 
	 * identificador do jogador.
	 * */
	public bool isMyRound(){
		return playerTime == playerRound;
	}
    
    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        Application.LoadLevel(Application.loadedLevel);

    }
	
    public void StartGame(){
		Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    
		if(PhotonNetwork.connected){
			photonView.RPC("PlayerConnected", PhotonTargets.All);
		}else{
			networkView.RPC("PlayerConnected",RPCMode.All);
		}
		
    }
	
	[RPC]
	void PlayerConnected(){
		if(isFirstPlayer){
			isFirstPlayer = false;
			playerTime = 0;
		}else{
		/*
			Camera.main.transform.position = new Vector3(0f,6.84f,-5.7f);
			Vector3 rot = transform.rotation.eulerAngles;
			Camera.main.transform.rotation = Quaternion.Euler(53.65f,rot.x,rot.y);	
			
			*/
		
			playerTime = 1;
			RequestshuffleParts();
		
		}
		//RequestshuffleParts();
	}
	
	
	void RequestshuffleParts(){
		//gameBoard.ShuffleParts();
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
       // Debug.LogWarning("OnFailedToConnectToPhoton");
    }
  
}
