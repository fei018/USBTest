﻿[app]
UsbNotifyService=usbnservice.exe
UsbNotifyAgent=usbnagent.exe
UsbNotifyAgentDesktop=usbntray.exe
UsbUpdateAgent=usbnUpdate.exe

[registry]
UsbFilterEnabled=false
UsbHistoryEnabled=true
AgentTimerMinute=5
AgentVersion=1.0.1
AgentKey=usbb50ae7e95f144874a2739e119e8791e1
UsbFilterDataUrl=http://localhost:5000/agent/usbFilterData
AgentSettingUrl=http://localhost:5000/Agent/AgentSetting
AgentUpdateUrl=http://localhost:5000/Agent/AgentUpdate
PostUsbRegisterRequestUrl=http://localhost:5000/agent/usbregisterquest
PostPerComputerUrl=http://localhost:5000/agent/PostPerComputer
PostPerUsbHistoryUrl=http://localhost:5000/agent/PostPerUsbHistory