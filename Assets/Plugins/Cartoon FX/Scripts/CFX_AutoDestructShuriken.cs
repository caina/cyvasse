using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;
	
	void Start()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			
			if(!particleSystem.IsAlive(true))
			{
				if(OnlyDeactivate)
					this.gameObject.SetActiveRecursively(false);
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
