﻿using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.VadimUnityDev.Robots_vs_Zombies_VR
{
    public class GameManager : Photon.PunBehaviour
    {
        #region Public Variables

        static public GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        [Tooltip("The prefab to use for representing the playerScripts")]
        public GameObject playerScripts;

        #endregion

        #region Photon Messages

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.name); // not seen if you're the player connecting

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

                LoadArena();
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.name); // seen when other disconnects

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

                LoadArena();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// asf
        /// </summary>
        void Start()
        {
            Instance = this;

            if (playerPrefab == null || playerScripts == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerScripts Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(this.playerScripts.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
                }
            }
        }

        private void Update()
        {
            // Do with controllers
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    LeaveRoom();
            //}
        }

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork: Loading Down Town Level.");
            PhotonNetwork.LoadLevel(1);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
    }
}
