private var currentTarget : GameObject=null;
public var rate : int = 10;
public var ballSet : boolean  = true;
public var displayText:String = "";

private var ball : GameObject;
private var plebe: GameObject;
public var isSetUp:boolean = true;
private var pieceSelected:GameObject = null;

function Start ()
{
	CreateGameBoard(10,10);
	//ball  = Instantiate(Resources.Load("GameBall"),Vector3(0,.5,0),Quaternion.identity);
	plebe = Instantiate(Resources.Load("peaces/Plebe"),Vector3(-1,.5,0),Quaternion.identity);
}

function OnGUI () {
	GUI.Label (Rect (0,0,300,50), displayText);
}

function SetTarget(target:GameObject){
	/* usar para movimentar as pecas
	if(currentTarget != null && target!=currentTarget){
		currentTarget.SendMessage("Deactivate");
	}
	currentTarget=target;
	ballSet=false;
	var travelTime : float = Vector3.Distance(ball.transform.position, target.transform.position)/rate;
	iTween.MoveBy(ball,iTween.Hash("x",target.transform.position.x-ball.transform.position.x,"easetype","easeinoutsine","time",travelTime));
	iTween.MoveBy(ball,iTween.Hash("z",target.transform.position.z-ball.transform.position.z,"time",travelTime,"delay",travelTime,"easetype","easeinoutsine","oncomplete","Reset","oncompletetarget",gameObject));
*/
}

function Reset(){
	ballSet=true;
}
	
function CreateGameBoard(cols:uint,rows:uint){
	var block : GameObject = Resources.Load("GameTile");
	
	for (var i = 0; i < cols; i++) {
		for (var j = 0; j < rows; j++) {
			var newBlock : GameObject = Instantiate(block,new Vector3(i,0,j),Quaternion.identity);
			newBlock.name="Block: " + i + "," + j;
			newBlock.SendMessage("SetGameboard",this);
			var blockColor : Color;
			if((j+i)%2 == 0){
				blockColor=Color.black;
			}else{
				blockColor=Color.white;
			}
			newBlock.renderer.material.color=blockColor;
			newBlock.transform.parent=transform;
		}
	}
	
	transform.position=Vector3(-(cols/2),0,-(rows/2));
}

/***
*-----------------------------------------------------
* FUNCOES PARA ADICIONAR PECAS NO CENARIO
* PRECISA:
* DESCOBRIR SE O CENENARIO ESTA CONCLUIDO
* LISTAR PECAS AINDA NAO POPULADA
* COLOCAR PECA SELECIONADA NO LOCAL INDICADO
* BOA SORTE.
*----------------------------------------------------
**/

function showPiecesToSet(tile:GameTile){
	Debug.Log(tile.name);
}