using UnityEngine;

//camera follow script for survey video system, when player enters collider area the camera of it
//gets activated and the camera follows the player
public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Camera cam;

    void Update ()
    {
        cam.transform.LookAt(player);
	}

    private void OnTriggerEnter(Collider other)
    {
        cam.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        cam.gameObject.SetActive(false);
    }
}
