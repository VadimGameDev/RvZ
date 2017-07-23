/* InstantVR Unity VR head controller
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.2
 * date: April 4, 2017
 * 
 * - Fix for running InstantVR without VR on other platforms
 */

using UnityEngine;
using UnityEngine.VR;

namespace IVR {

    public class IVR_UnityVRHead : IVR_Controller {
        [HideInInspector]
        public GameObject cameraRoot;
        [HideInInspector]
        private Transform cameraTransform;

        [HideInInspector]
        private Vector3 neck2eyes;

        [HideInInspector]
        public bool vrTracking = false;
        [HideInInspector]
        public bool positionalTracking = false;
        [HideInInspector]
        private bool originOnFloor = false;

#if INSTANTVR_ADVANCED
#if IVR_KINECT
        [HideInInspector]
        private IVR_Kinect2Head kinect2Head;
#endif
#endif

        public void Start() {
            // This dummy code is here to ensure the checkbox is present in editor
        }

        public override void StartController(InstantVR ivr) {
            if (extension == null)
                extension = ivr.GetComponent<IVR_UnityVR>();

            extension.present = VRDevice.isPresent;

            Camera camera = CheckCamera();
            if (camera != null) {
                cameraTransform = camera.transform;
                neck2eyes = HeadUtils.GetNeckEyeDelta(ivr);

                DeterminePlatform();

                if (vrTracking) {
                    if (!originOnFloor)
                        cameraRoot.transform.position = transform.position;
                    cameraRoot.transform.rotation = ivr.transform.rotation;

                    cameraTransform.SetParent(cameraRoot.transform, false);
                }

                base.StartController(ivr);

#if INSTANTVR_ADVANCED
#if IVR_KINECT
                kinect2Head = GetComponent<IVR_Kinect2Head>();
#endif
#endif
            }
        }

        private Camera CheckCamera() {
            Camera camera = transform.GetComponentInChildren<Camera>();
            if (camera == null) {
                GameObject cameraObj = new GameObject("First Person Camera");
                cameraObj.transform.SetParent(transform);
                camera = cameraObj.AddComponent<Camera>();
                camera.nearClipPlane = 0.05F;
            }

            return camera;
        }

        private void DeterminePlatform() {
            if (!VRSettings.enabled) {
                vrTracking = false;
                return;
            }

            vrTracking = true;
            switch (VRSettings.loadedDeviceName) {
                case "Oculus":
                    positionalTracking = true;
                    originOnFloor = false;
                    break;
                case "OpenVR":
                    positionalTracking = true;
                    originOnFloor = true;
                    break;
                default:
                    positionalTracking = false;
                    break;
            }
        }

        public override void UpdateController() {
            if (!extension.present || !enabled)
                return;

            tracking = true;
            UpdateUnityVR();
        }

        private void UpdateUnityVR() {
            CalculateCameraRoot();
            SetHeadTargets();
        }


        private void CalculateCameraRoot() {
            if (positionalTracking && originOnFloor)
                cameraRoot.transform.localPosition = extension.trackerPosition;
        }

        private void SetHeadTargets() {
                transform.rotation = cameraTransform.rotation;
                if (positionalTracking)
                    transform.position = cameraTransform.position + cameraTransform.rotation * -neck2eyes;
#if INSTANTVR_ADVANCED
#if IVR_KINECT
                else {
                    if (kinect2Head != null && kinect2Head.isTracking()) {
                        transform.position = kinect2Head.position;
                    }
                }
#endif
#endif
        }

        public override void OnTargetReset() {
            InputTracking.Recenter();
        }
    }
}