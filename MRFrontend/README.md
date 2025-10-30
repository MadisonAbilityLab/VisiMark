# VisiMark: an Augmented Reality Interface to Support Landmark Perception

This folder contains the Augmented Reality application that augments landmarks for **people with low vision**, running on a mixed reality headset (i.e., Microsoft HoloLens 2 for *VisiMark*).

---

## Project Description

In this project, we incorporated the [MixedReality-QRCode-Sample](https://github.com/microsoft/MixedReality-QRCode-Sample/tree/OpenXR) to hard-code landmarks in a local campus building.  
You can refer to:

- `Assets/Scenes/MyMainScene.unity` — serves as the main experimental scene used in the user study.

---

## Deployment Instructions

### 1. Install Mixed Reality Tools
Follow Microsoft’s official setup guide: [Install the tools for Mixed Reality development](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/install-the-tools?tabs=unity)

After installation, use the **Mixed Reality Feature Tool.exe** to add the following components:
```bash
"Platform Support/Mixed Reality OpenXR Plugin"
"Mixed Reality Toolkit/Mixed Reality Toolkit Foundation"
```

### 2. Install the Recommended Unity Version

Use Unity **2021.3.31f1** (or a compatible LTS version).
Make sure the Universal Windows Platform (UWP) build support module is included.

### 3. Add the QR Code Tracking Package

Download and import the QR Code NuGet package as described in the [MixedReality-QRCode-Sample](https://github.com/microsoft/MixedReality-QRCode-Sample/tree/OpenXR) documentation.

Note:
For Windows Mixed Reality headsets, QR code tracking on desktop PCs is only supported on Windows 10 Version 2004 and higher.

### Notes

The AR application communicates with the companion phone application via WebSocket for real-time experiment control and logging. Please ensure both the HoloLens 2 and the phone are connected to the same local network.
