using UnityEngine;
using System.Collections;

public class GameTile : MonoBehaviour{
	
	private Color currentColor;
	private Vector3 currentPosition;
	public Board gameBoard;
	bool isActive;
	
	private string pieceType = "";
	private GameObject fieldObject = null;
	public GameObject onMePiece = null;
	//posicao na matri
	public int x;
	public int z;
	public int onMeId = 99999;
	public bool isTileOption = false;
	
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
		renderer.material.color=Color.red;
	//	photonView.RPC("MoveBall", PhotonTargets.All,gameObject);
		//gameBoard.SetTarget(gameObject);
		//SendMessageUpwards("SetTarget",gameObject);	
		iTween.MoveTo(this.gameObject,currentPosition,.4f);
	}
	
	public void isOption(){
		isTileOption = true;
	//	Debug.Log("me selecionou! "+x.ToString()+" - "+z.ToString());
		iTween.MoveTo(gameObject, new Vector3(currentPosition.x,.5f,currentPosition.z),.2f);
	}
	
	public void hideOptions(){
		if(isTileOption){
			isTileOption = false;
			Debug.Log("to saindo");
			iTween.MoveTo(gameObject,currentPosition,.4f);
		}
	}
	
	void OnMouseEnter(){
		if(!isActive){
			if(!gameBoard.ballSet){
				renderer.material.color=Color.yellow;
			}else{
				renderer.material.color=Color.green;
			}
			transform.position=new Vector3(currentPosition.x,.5f,currentPosition.z);
		}
	}
	

	
	void OnMouseExit(){
		if(!isActive){
			iTween.ColorTo(gameObject,currentColor,.4f);
			iTween.MoveTo(gameObject,currentPosition,.4f);
		}
	}
	
	void OnMouseDown(){
		
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
	
	void changeType(string type){
		if(pieceType.Equals("montain")){
			//destruir montanha
			if(fieldObject!=null){
				Destroy(fieldObject);
				fieldObject = null;
			}
		}
		
		//if montain, instantiate obj	
		if(type.Equals("montain")){
			
		}
	}
	
}

