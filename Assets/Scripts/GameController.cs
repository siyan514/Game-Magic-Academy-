using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject playerPre;
    private MapController mapController;

    private void Start()
    {
        mapController = GetComponent<MapController>();
        mapController.initMap(8, 3, 20);
        GameObject player = Instantiate(playerPre);
        player.transform.position = mapController.GetPlayerPos();
    }
}
