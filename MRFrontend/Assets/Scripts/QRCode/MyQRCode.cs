using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class MyQRCode : MonoBehaviour, IMixedRealityPointerHandler
    {
        public delegate void sendMessage(String str);
        public static event sendMessage MessageSender;

        public Microsoft.MixedReality.QR.QRCode qrCode;
        private GameObject qrCodeCube;

        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        private long lastTimeStamp = 0;
        SpatialGraphNodeTracker tracker;


        void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;
            qrCodeCube = gameObject.transform.Find("Cube").gameObject;

            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute, out uriResult))
            {
                validURI = true;
            }
            tracker = GetComponent<SpatialGraphNodeTracker>();
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                PhysicalSize = qrCode.PhysicalSideLength;
                //MessageSender("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize +" QRData = " + CodeText);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;


                qrCodeCube.transform.position = tracker.pose.position;
                qrCodeCube.transform.rotation = tracker.pose.rotation;
            }
        }

        void Update()
        {
            UpdatePropertiesDisplay();
            if (launch)
            {
                launch = false;
                LaunchUri();
            }
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (validURI)
            {
                launch = true;
            }
            MessageSender("MyQRCode: Clicked!!");
        }
    }
}
