using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
	
	public Board gameBoard;
	public int piecesMaxMoves=1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown(){
		//verificar se a peca eh minha
		Debug.Log(this.gameObject.name);
		gameBoard.selectPiece(int.Parse(this.gameObject.name));
	}
	
	void SetGameboard(Board gameBoard){
		this.gameBoard=gameBoard;
	}
}
