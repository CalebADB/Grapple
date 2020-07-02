﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace masterFeature
{
    public class MainCamera : MonoBehaviour
    {
        public Camera camera;
        private Player_Controller player;
        private Player_Cursor cursor;

        private Vector2 cameraPos;
        private Vector2 playerPos;
        private Vector2 cursorPos;

        public bool updateVariables;

        [Range(-20f, 0f)]
        public float cameraDistance;

        [Range(0.01f, 10f)]
        public float cameraMaxRadius;
        [Range(0.01f, 4f)]
        public float cameraCursorPullFactor;

        [Range(0.0001f, 0.5f)]
        public float cameraFollowFactor;

        private float radialAndSeverityRatio;
        private float flatteningValue;


        private void Awake()
        {
            camera = this.GetComponent<Camera>();

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 1)
            {
                player = players[0].GetComponentInChildren<Player_Controller>();
            }
            else { Debug.Log("More then one object with player tag"); };

            GameObject[] cursors = GameObject.FindGameObjectsWithTag("Cursor");
            if (cursors.Length == 1)
            {
                cursor = cursors[0].GetComponentInChildren<Player_Cursor>();
            }
            else { Debug.Log("More then one object with cursor tag"); };
        }

        private void Start()
        {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraDistance);
            updateCameraVariables();
        }

        // Update is called once per frame
        void Update()
        {
            if (updateVariables)
            {
                updateCameraVariables();
                updateVariables = false;
            }
        }

        private void FixedUpdate()
        {
            followPlayer();
        }

        void followPlayer()
        {
            cameraPos = this.transform.position;
            playerPos = player.transform.position;
            cursorPos = cursor.transform.position;

            Vector2 playerToCursor = cursor.transform.position - player.transform.position;
            Vector2 unitVectPlayerToCursor = playerToCursor.normalized;
            float distPlayerToCursor = playerToCursor.magnitude;

            // Complicated function. See desmos file for more info: https://www.desmos.com/calculator/v6itjtqy3o
            float distPlayerToAnchor = cameraCursorPullFactor * ((flatteningValue * Mathf.Log(1 + (Mathf.Pow(2, distPlayerToCursor / radialAndSeverityRatio)))) + distPlayerToCursor + radialAndSeverityRatio);
            if (distPlayerToAnchor > (3 * distPlayerToCursor /4))
            {
                distPlayerToAnchor = (3 * distPlayerToCursor /4);
            }
            Vector2 cameraTarget = playerPos + (distPlayerToAnchor * unitVectPlayerToCursor);
            Debug.DrawLine(playerPos, cameraTarget, Color.magenta, 0.001f);



            Vector3 newCameraPos = (cameraTarget * cameraFollowFactor) + (cameraPos * (1f - cameraFollowFactor));
            newCameraPos.z = cameraDistance;
            newCameraPos = newCameraPos;
            this.transform.position = newCameraPos;
        }

        public void updateCameraVariables()
        {
            radialAndSeverityRatio = cameraMaxRadius/cameraCursorPullFactor;
            flatteningValue = -(radialAndSeverityRatio/(Mathf.Log(2)));
        }
    }
}
