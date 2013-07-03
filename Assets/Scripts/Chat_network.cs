using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chat_network : MonoBehaviour {

 	public static Chat_network SP;
    public List<string> messages = new List<string>();

    private int chatHeight = (int)140;
    private Vector2 scrollPos = Vector2.zero;
    public string chatInput = "";
    private float lastUnfocusTime = 0;
	
	public GUISkin skin;
    
	void Awake()
    {
        SP = this;
    }

    void OnGUI()
    {        
		GUI.skin = skin;
        GUI.SetNextControlName("");

        GUILayout.BeginArea(new Rect(5, (Screen.height-10) - chatHeight, 200, chatHeight));
        
        //Show scroll list of chat messages
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUI.color = Color.red;
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();
        GUI.color = Color.white;

        //Chat input
        GUILayout.BeginHorizontal(); 
        GUI.SetNextControlName("ChatField");
    chatInput = GUILayout.TextField(chatInput, GUILayout.MinWidth(200));
       
        if (Event.current.type == EventType.keyDown && Event.current.character == '\n'){
            if (GUI.GetNameOfFocusedControl() == "ChatField")
            {                
                SendChat();
                lastUnfocusTime = Time.time;
                GUI.FocusControl("");
                GUI.UnfocusWindow();
            }
            else
            {
                if (lastUnfocusTime < Time.time - 0.1f)
                {
                    GUI.FocusControl("ChatField");
                }
            }
        }

        //if (GUILayout.Button("SEND", GUILayout.Height(17)))
         //   SendChat(PhotonTargets.All);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    

        GUILayout.EndArea();
    }

    public static void AddMessage(string text)
    {
        SP.messages.Add(text);
        if (SP.messages.Count > 15)
            SP.messages.RemoveAt(0);
    }


    [RPC]
    public void _sendChatMessage(string text)
    {
        AddMessage(text);
    }
	
    public void SendChat()
    {
        if (chatInput != "")
        {
            networkView.RPC("_sendChatMessage",RPCMode.All,chatInput);
            chatInput = "";
        }
    }

  

    void OnLeftRoom()
    {
        this.enabled = false;
    }

    void OnJoinedRoom()
    {
        this.enabled = true;
    }
    void OnCreatedRoom()
    {
        this.enabled = true;
    }
}
