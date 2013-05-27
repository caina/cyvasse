private var currentColor : Color;
private var currentPosition : Vector3;
private var gameBoard : GameBoardJS;
private var isActive : boolean;
public var landType;

function Start(){
	currentColor=renderer.material.color;
	currentPosition=transform.position;
	this.landType = "grass";
}

function SetGameboard(gameBoard : GameBoardJS){
	this.gameBoard=gameBoard;
}

function Deactivate(){
	isActive=false;
	iTween.ColorTo(gameObject,currentColor,.4);
}

function Activate(){
  	//verificar se o jogador pode fazer o movimento
	isActive=true;
	renderer.material.color=Color.red;
	SendMessageUpwards("SetTarget",gameObject);	
	iTween.MoveTo(gameObject,currentPosition,.4);
}

function OnMouseEnter(){
	 Debug.Log ("Ativando: "+name+" do tipo: "+this.landType);
	this.gameBoard.displayText = "ativando: "+name+" do tipo: "+this.landType;
	if(!isActive){
		if(!gameBoard.ballSet){
			renderer.material.color=Color.yellow;
		}else{
			renderer.material.color=Color.green;
		}
		transform.position=new Vector3(currentPosition.x,.5,currentPosition.z);
	}
}

function OnMouseExit(){
	if(!isActive){
		iTween.ColorTo(gameObject,currentColor,.4);
		iTween.MoveTo(gameObject,currentPosition,.4);
	}
}

function OnMouseDown(){
	if(gameBoard.ballSet){
		Activate();
	}
}