using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable All

public class ResetDemo : MonoBehaviour {

	void Update () {
		if(Input.GetKeyDown("r")){
			SceneManager.LoadScene(0);
		}
	}
}
