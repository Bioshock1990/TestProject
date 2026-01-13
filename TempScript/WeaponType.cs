#pragma warning disable
using UnityEngine;
using System.Collections.Generic;

public class WeaponType : MonoBehaviour {	
	public Weapon Type;
	
	private void Update() {
		if(Type != null) {
			Debug.Log(Type.Damage);
		}
	}
}

