using UnityEngine;
using System.Collections;

public class GameTile : MonoBehaviour{
	
	private Color currentColor;
	private Vector3 currentPosition;
	public Board gameBoard;
	bool isActive;
	
	private string pieceType = "";
	public GameObject fieldObject = null;
	public GameObject onMePiece = null;
	//posicao na matri
	public int x;
	public int z;
	public int onMeId = 99999;
	public bool isTileOption = false;
	
	public Texture defaultTexture;
	
	void Start(){
		currentColor=renderer.material.color;
		currentPosition=transform.position;
	}
	
	void SetGameboard(Board gameBoard){
		this.gameBoard=gameBoard;
	}
	
	void Deactivate(){
		isActive=false;
		iTween.ColorTo(gameObject,currentColor,.4f);
	}
	
	void Activate(){
		isActive=true;
		
		//photonView.RPC("moveBall",PhotonTargets.All);
		gameBoard.SetTarget(this.gameObject);
		//renderer.material.color=Color.red;
	//	photonView.RPC("MoveBall", PhotonTargets.All,gameObject);
		//gameBoard.SetTarget(gameObject);
		//SendMessageUpwards("SetTarget",gameObject);	
		iTween.MoveTo(this.gameObject,currentPosition,.4f);
	}
	
	public void isOption(){
		if(!isFrozen()){
			isTileOption = true;
		//	Debug.Log("me selecionou! "+x.ToString()+" - "+z.ToString());
			iTween.MoveTo(gameObject, new Vector3(currentPosition.x,.5f,currentPosition.z),.2f);
		}
	}
	
	public void hideOptions(){
		if(isTileOption){
			isTileOption = false;
			Debug.Log("to saindo");
			iTween.MoveTo(gameObject,currentPosition,.4f);
		}
	}
	
	void OnMouseEnter(){
		if(gameBoard.gameManager.isOnMountPhase()){
			if(gameBoard.groundOfSelected != null){
				if(onMeId==99999){
					gameBoard.rpcChangeGrounds(x,z);
				}	
			}
		}
		
		
		if(!isActive && !isFrozen()){
			if(!gameBoard.ballSet){
				renderer.material.color=Color.yellow;
			}else{
				renderer.material.color=Color.green;
			}
			transform.position=new Vector3(currentPosition.x,.5f,currentPosition.z);
		}
	}
	
	void OnMouseExit(){
		if(!isActive && !isFrozen()){
			iTween.ColorTo(gameObject,currentColor,.4f);
			iTween.MoveTo(gameObject,currentPosition,.4f);
		}
	}
	
	void OnMouseDown(){
		
		//movimentar peca na faze de construcao
		if(gameBoard.gameManager.isOnMountPhase()){
			if(hasGround() && gameBoard.groundOfSelected == null){
				gameBoard.groundOfSelected = gameObject;
			}else{
				gameBoard.groundOfSelected = null;
			}
		}
		
		if(gameBoard.hasPieceSelected()){
			//mover peca selecionada
			if(gameBoard.ballSet){
				Activate();
			}
		}
	}
	
	void OnTriggerEnter(Collider collision) {
		Debug.Log("colidi");
		if(collision.gameObject.tag=="BoardPiece"){
			this.onMePiece = collision.gameObject;	
		}  
    }
	
	void OnTriggerExit(Collider other) {
       if(other.gameObject.tag=="BoardPiece"){
			this.onMePiece = null;
		}
    }
	
	public bool hasGround(){
		return !pieceType.Equals("");
	}
	
	public bool isFrozen(){
		return pieceType.Equals("montain");
		
	}
		
	public void changeType(string type){
		if(pieceType.Equals("montain")){
			//destruir montanha
			if(fieldObject!=null){
				Destroy(fieldObject);
				fieldObject = null;
			}
		}
		if(pieceType.Equals("water") || pieceType.Equals("forest")){
			gameObject.renderer.material.SetTexture("_MainTex",defaultTexture);	
		}
		
		//if montain, instantiate obj	
		if(type.Equals("montain")){
			Vector3 pos = transform.position;
			pos.y = 0.1203094f;
			
			GameObject montain = (GameObject) Instantiate(Resources.Load("Montain"), pos, Quaternion.Euler(270, 0, 0));
			fieldObject = montain;
		}else if(type.Equals("water")){
			gameObject.renderer.material.SetTexture("_MainTex",(Texture)Resources.Load("textures/water"));		
		}else if(type.Equals("forest")){
			gameObject.renderer.material.SetTexture("_MainTex",(Texture)Resources.Load("textures/forest"));		
		}
		
		
		pieceType = type;
	}
	
	public void migrateTypes(GameObject fromTile){
		changeType(fromTile.GetComponent<GameTile>().pieceType);
		fromTile.GetComponent<GameTile>().changeType("");
	}
	
}

