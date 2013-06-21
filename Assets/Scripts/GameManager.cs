using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
	public bool isFirstPlayer = true;
	private Chat chat;
	public Board gameBoard;
	
	private int playerId = 1;
	public int playerRound = 0;
	public int playerTime;
	
	public bool isLocal = false;
	private PhotonView photonView;
	private bool onMountPhase = true;
	
	private int playersReady = 0;
	private bool imReady = false;
	public GameObject gameGate;
	private bool canPlay = false;
	
	
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
			gameBoard.playerNumber = 1;
		}else{
		/*
			Camera.main.transform.position = new Vector3(0f,6.84f,-5.7f);
			Vector3 rot = transform.rotation.eulerAngles;
			Camera.main.transform.rotation = Quaternion.Euler(53.65f,rot.x,rot.y);	
			
			*/
			gameBoard.playerNumber = 2;
			playerTime = 1;
			RequestshuffleParts();
		
		}
		//RequestshuffleParts();
	}
	
	
	void RequestshuffleParts(){
		//gameBoard.ShuffleParts();
		SendChatMessage("embaralharemos as pecas! preparem-se!");
		
	}
	
	private int playerCount = 1;
    void OnPlayerConnected(NetworkPlayer player) {
        playerCount++;
		playerId++;
		if(playerCount==2){
			if(PhotonNetwork.connected){
				photonView.RPC("letsBegin", PhotonTargets.All);
			}else{
				networkView.RPC("letsBegin",RPCMode.All);
			}	
		}
    }
	
	[RPC]
	void letsBegin(){
		canPlay = true;
		Camera.main.farClipPlane = 1000; 
		if(playerId==2){
			Camera.main.transform.position = new Vector3(8.729182f,7.38392f,-0.0770278f);
			Camera.main.transform.rotation = Quaternion.Euler(30.10073f, 272.0699f, 359.6782f);
		}
		// YUNO funciona?!!?!?!?
		//Camera.main.gameObject.GetComponent<MouseOrbit>().enabled = true;
		
	}
	
    void OnGUI()
    {
		if(!canPlay & hasConnection()){
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
		        GUILayout.Label("Aguardando o oponente");
	        GUILayout.EndArea();
		}
		if(!imReady && hasConnection() && canPlay){
			GUILayout.BeginArea(new Rect(0, 0, 150, 500));
				GUILayout.BeginHorizontal();
					if(GUILayout.Button ("Estou Pronto")){
						imReady = true;
						playerIsReady();
					}
		        GUILayout.EndHorizontal();
	        GUILayout.EndArea();	
		}
        
		
		if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave& QUIT"))
        {
            PhotonNetwork.LeaveRoom();
        }
		
		
		
    }
	
	public bool hasConnection(){
		if(PhotonNetwork.connected){
			return true;
		}else{
			return !(Network.peerType == NetworkPeerType.Disconnected);
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
	
	public bool isOnMountPhase(){
		return onMountPhase;
	}
	
	void playerIsReady(){
		if(PhotonNetwork.connected){
			photonView.RPC("_playerIsReady", PhotonTargets.All);
		}else{
			networkView.RPC("_playerIsReady",RPCMode.All);
		}
	}
	
	[RPC]
	void _playerIsReady(){
		playersReady++;
		if(playersReady == 2){
			Destroy(gameGate);
			onMountPhase = false;
		}
	}
	
	public int getPlayerId(){
		return playerId;	
	}
  
}
