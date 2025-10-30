# Phone Application for Customization and Debugging

This folder contains the phone application built with React Native. It is designed to connect with the AR frontend running on a mixed reality headset (i.e., Microsoft HoloLens 2 for *VisiMark*).

The experimenter can use this mobile app to communicate with the headset during the experiment. For example, in *VisiMark*'s evaluation, experimenters sent control commands to customize the font size and color using the *Wizard of Oz* method (Dahlbäck et al., 1993), and received real-time debugging logs from the headset.

---

## Setup Instructions

### 1. Configure the Connection
- Open `src/Home.jsx`
- Replace the value of `socketUrl` with the **IP address of the headset**
- Ensure that **both the phone and the headset are on the same local network**

### 2. Install Dependencies
```bash
npm install
```

### 3. Start the Application
```bash
npm start
```

### 4. Launch on Your Phone
Scan the QR code displayed in the terminal with your phone to open the app in <a href="https://expo.dev/go">Expo Go</a>.

## References
- Dahlbäck, N., Jönsson, A., & Ahrenberg, L. (1993, February). Wizard of Oz studies: why and how. In Proceedings of the 1st international conference on Intelligent user interfaces (pp. 193-200).
