set powerManager = CreateObject("PowerManagement.PowerManagementClient")

a = powerManager.LastSleepTime()

WScript.Echo(a)