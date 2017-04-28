/* 
 * How to registry COM object:
 * Worked with next RegAsm path (win 10) c:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe
 * > regasm "dll path" /codebase /tlb
 */

using System;
using System.Runtime.InteropServices;

namespace PowerManagement
{
  [Guid("D3816EB5-D5E7-4B79-87CE-692E161420D9"),
    InterfaceType(ComInterfaceType.InterfaceIsDual),
    ComVisible(true)]
  public interface IPowerManagementClient
  {
    long LastSleepTime();
    long LastWakeTime();
    SYSTEM_POWER_INFORMATION PowerInformation();
    SYSTEM_BATTERY_STATE BatteryState();
    void SaveHibenationFile();
    void RemoveHibenationFile();
    bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
  }

  internal enum InformationLevel
  {
    LastSleepTime = 15,
    LastWakeTime = 14,
    PowerInformation = 12,
    BatteryState = 5
  }

  [Guid("617F7219-33D3-4C4C-9C7C-366556245413"),
    ClassInterface(ClassInterfaceType.None),
    ComVisible(true)]
  public class PowerManagementClient : IPowerManagementClient
  {
    public long LastSleepTime()
    {
      long sleepTime;
      PowerManagementInterop.LastSleepOrWakeTime((int)InformationLevel.LastSleepTime, IntPtr.Zero, 0, out sleepTime, Marshal.SizeOf(typeof(long)));
      return sleepTime;
    }

    public long LastWakeTime()
    {
      long sleepTime;
      PowerManagementInterop.LastSleepOrWakeTime((int)InformationLevel.LastWakeTime, IntPtr.Zero, 0, out sleepTime, Marshal.SizeOf(typeof(long)));
      return sleepTime;
    }

    public SYSTEM_POWER_INFORMATION PowerInformation()
    {
      SYSTEM_POWER_INFORMATION powerInformation;
      var retval = PowerManagementInterop.PowerInformation((int)InformationLevel.PowerInformation, IntPtr.Zero, 0,
          out powerInformation, Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
      );

      return powerInformation;
    }

    public SYSTEM_BATTERY_STATE BatteryState()
    {
      SYSTEM_BATTERY_STATE batteryState;
      var retval = PowerManagementInterop.BatteryState((int)InformationLevel.BatteryState, IntPtr.Zero, 0,
          out batteryState, Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))
      );

      return batteryState;
    }

    public void SaveHibenationFile()
    {
      PowerManagementInterop.ToggleHibernationFile(10, true, Marshal.SizeOf(typeof(bool)), IntPtr.Zero, 0);
    }

    public void RemoveHibenationFile()
    {
      PowerManagementInterop.ToggleHibernationFile(10, false, Marshal.SizeOf(typeof(bool)), IntPtr.Zero, 0);
    }

    public bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent)
    {
      return PowerManagementInterop.SetSuspendState(hibernate, forceCritical, disableWakeEvent);
    }
  }
}