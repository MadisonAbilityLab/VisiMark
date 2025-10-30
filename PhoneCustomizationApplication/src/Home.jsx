import React, { useEffect, useState } from 'react';
import { Button, StyleSheet, Text, TextInput, View, Modal, ScrollView } from "react-native";
import { Picker } from '@react-native-picker/picker';

const socketUrl = 'ws://headset_ip_address:8012/';


export default function Home() {
    const [controlSocket, setControlSocket] = useState(null);
    const [debugSocket, setDebugSocket] = useState(null);
    const [monitoringSocket, setMonitoringSocket] = useState(null);
    const [messages, setMessages] = useState(['Start!','Start!','Start!','Start!','Start!']);
    const [debugError, setDebugError] = useState(null);
    const [headError, setHeadError] = useState(null);
    const [headData, setHeadData] = useState(['Start!','Start!','Start!','Start!','Start!']);
    const [controlError, setControlError] = useState(null);
    const [controlResponse, setControlResponse] = useState(null); 
    const [selectedStage, setSelectedStage] = useState('8');
    const [lastSelectedStage, setLastSelectedStage] = useState('8');
    const [augOn,setAugOn] = useState(true);
    const [CHOn,setCHOn] = useState(true);
    const [iconsOn,setIconsOn] = useState(true);
    const [signBoardOn,setSignBoardOn] = useState(true);

    useEffect(() => {

      const newControlSocket = new WebSocket(socketUrl+'control');
      newControlSocket.onopen = () => {
          console.log('Control socket connected to server');
      };
      newControlSocket.onmessage = (event) => {
          // console.log('Control socket received message:', event.data);
          setControlResponse(event.data);
      };
      newControlSocket.onerror = (error) => {
          console.error('Control socket error:', error);
          setControlError(error);
      };
      newControlSocket.onclose = () => {
          console.log('Control socket disconnected from server');
      };
      setControlSocket(newControlSocket);

      
      const newDebugSocket = new WebSocket(socketUrl+'debug');
      newDebugSocket.onopen = () => {
        console.log('Debug socket connected to server');
      };
      newDebugSocket.onmessage = (event) => {
          // console.log('Debug socket received message:', event.data);
          const updatedMessages = [...messages, event.data];
          if (updatedMessages.length > 5) {
              setMessages(prevMessages => [...prevMessages, event.data].slice(updatedMessages.length - 5));
          } else {
              setMessages(prevMessages => [...prevMessages, event.data]);
          }
      };
      newDebugSocket.onerror = (error) => {
        console.error('Debug socket error:', error);
        setDebugError(error);
      };
      newDebugSocket.onclose = () => {
        console.log('Debug socket disconnected from server');
      };
      setDebugSocket(newDebugSocket);

      
      const newMonitoringSocket = new WebSocket(socketUrl+'monitoring');
      newMonitoringSocket.onopen = () => {
        console.log('Monitoring socket connected to server');
      };
      newMonitoringSocket.onmessage = (event) => {
          // console.log('Monitoring socket received message:', event.data);
          // setHeadData(event.data);
          const updatedMessages = [...messages, event.data];
          if (updatedMessages.length > 5) {
            setHeadData(prevMessages => [...prevMessages, event.data].slice(updatedMessages.length - 5));
          } else {
            setHeadData(prevMessages => [...prevMessages, event.data]);
          }
      };
      newMonitoringSocket.onerror = (error) => {
        console.error('Monitoring socket error:', error);
        setHeadError(error);
      };
      newMonitoringSocket.onclose = () => {
        console.log('Monitoring socket disconnected from server');
      };
      setMonitoringSocket(newMonitoringSocket);

      return () => {
        newControlSocket.close();
        newDebugSocket.close();
        newMonitoringSocket.close();
      };
  }, []);

  
  useEffect(() => {
    const intervalId = setInterval(() => {
      fetchDebugData();
      fetchHeadData();
    }, 500);
    return () => clearInterval(intervalId);
  }, [monitoringSocket,debugSocket]); 

  const sendMessage = (message) => {
    if(message=='a'){
      setAugOn(prev=>!prev);
    }
    else if(message=='c'){
      setCHOn(prev=>!prev);
    }
    else if(message=='i'){
      setIconsOn(prev=>!prev);
    }
    else if(message=='s'){
      setSignBoardOn(prev=>!prev);
    }
    else if(message=='7'){
      setAugOn(false);
      setCHOn(false);
      setIconsOn(false);
      setSignBoardOn(false);
      setLastSelectedStage(message);
    }
    else if(message=="1" || message=="2"|| message=="3"|| message=="4"|| message=="5"|| message=="6"){
      if(lastSelectedStage == "7"){
        setAugOn(true);
        setCHOn(true);
        setIconsOn(true);
        setSignBoardOn(true);
      }
      setLastSelectedStage(message);
    }
    if (controlSocket && controlSocket.readyState === WebSocket.OPEN) {
      controlSocket.send(message);
    } 
    else {
      console.error('Control socket not connected');
    }
  };

  const fetchHeadData = () => {
      if (monitoringSocket && monitoringSocket.readyState === WebSocket.OPEN) {
        monitoringSocket.send('1');
      } 
      else {
        console.error('Monitoring socket not connected');
      }
  };
  const fetchDebugData = () => {
      if (debugSocket && debugSocket.readyState === WebSocket.OPEN) {
        debugSocket.send('1');
      } 
      else {
        console.error('Debug socket not connected');
      }
  };

  return (
    <View style={styles.container}>
        <ScrollView style={styles.halfUpPage}>
            {messages.map((message, index) => (
                <Text key={index} style={styles.message}>Message {index}: {message}</Text>
            ))}
        </ScrollView>
        <ScrollView style={styles.halfDownPage} horizontal={false} vertical={false}>
            <Button title="Adjust Position" onPress={() => sendMessage('ctrl')} />
            <View style={styles.pickerContainer}>
                <Button title="Aug" onPress={() => sendMessage('a')} />
                <Button title="CH" onPress={() => sendMessage('c')} />
                <Button title="Icons" onPress={() => sendMessage('i')} />
                <Button title="SignBoard" onPress={() => sendMessage('s')} />
            </View>
            <View style={styles.pickerContainer}>
              <Text style={styles.ifshow}>{augOn ? 'True' : 'False'}</Text>
              <Text style={styles.ifshow}>{CHOn ? 'True' : 'False'}</Text>
              <Text style={styles.ifshow}>{iconsOn ? 'True' : 'False'}</Text>
              <Text style={styles.ifshow}>{signBoardOn ? 'True' : 'False'}</Text>
            </View>
            <View style={styles.pickerContainer}>
                <ScrollView style={{ flex: 1 }}>
                    <Picker
                        selectedValue={selectedStage}
                        style={styles.picker}
                        onValueChange={(itemValue, itemIndex) =>
                            setSelectedStage(itemValue)
                        }>
                        <Picker.Item label="Font Size" value="1" />
                        <Picker.Item label="Font Color" value="2" />
                        <Picker.Item label="Icon Size" value="3" />
                        <Picker.Item label="SignBoard Size" value="4" />
                        <Picker.Item label="SignBoard Icon Size" value="5" />
                        <Picker.Item label="Show Aug 1 By 1" value="7" />
                        <Picker.Item label="None" value="8" />
                    </Picker>
                </ScrollView>
            </View>
            <View style={styles.pickerContainer}>
                <Button title="Set Stage" onPress={() => sendMessage(selectedStage)} />
                <Button title="Previous" onPress={() => sendMessage('left')} />
                <Button title="Next" onPress={() => sendMessage('right')} />
            </View>
            <Button title="Save Customization" onPress={() => sendMessage('space')} />
            <ScrollView style={styles.scrollResponse}>
              
                {headData.map((head, index) => (
                    <Text key={index} style={styles.headData}>Watch {index}: {head}</Text>
                ))}
                <Text style={styles.response}>Control Response: {controlResponse}</Text>
                <Text style={styles.error}>Control Error: {controlError}</Text>
                <Text style={styles.error}>Get Debug Error: {debugError}</Text>
                <Text style={styles.error}>Get Head Data Error: {headError}</Text>
            </ScrollView>
        </ScrollView>
    </View>
  );
}
const styles = StyleSheet.create({
  container: {
      flex: 1,
      justifyContent: 'center',
      padding: 20,
  },
  halfUpPage: {
      borderWidth: 1,
      borderColor: 'gray',
    marginTop: 40,
    marginBottom: 20,
    padding: 10,
  },
  scrollResponse:{
      marginTop: 30,
      borderWidth: 1,
      borderColor: 'gray',
      padding: 10,
  },
  divider: {
      height: 1,
      backgroundColor: 'gray',
      marginBottom: 10,
      marginTop: 0,
  },
  ifshow:{
      flex: 1,
    fontSize: 16,
    color: 'purple',
  },
  halfDownPage: {
    marginBottom: 20,
  },
  pickerContainer: {
      flexDirection: 'row',
      alignItems: 'center',
    },
  message: {
      textAlign: 'left'
  },
  input: {
      width: '100%',
      marginBottom: 20,
      padding: 10,
      borderWidth: 1,
      borderColor: '#ccc',
  },
  response: {
      color: 'green',
      marginTop: 20,
  },
  headData:{
      color: 'brown',
  },
  error: {
    color: 'red',
    marginTop: 10,
    marginBottom: 0,
    paddingBottom: 0,
  },
  picker: {
      flex: 1,
      marginRight: 1, 
      fontSize: 8,
  },
  modalContainer: {
      flex: 1,
      justifyContent: 'center',
      alignItems: 'center',
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
    },
    modalContent: {
      backgroundColor: 'white',
      padding: 20,
      borderRadius: 10,
      alignItems: 'center',
    },
});