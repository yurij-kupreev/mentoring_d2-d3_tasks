using System;
using System.Runtime.InteropServices;

namespace PowerManagement
{
  public struct SYSTEM_POWER_INFORMATION
  {
    public uint MaxIdlenessAllowed;
    public uint Idleness;
    public uint TimeRemaining;
    public byte CoolingMode;
  }

  public struct SYSTEM_BATTERY_STATE
  {
    public bool AcOnLine;
    public bool BatteryPresent;
    public bool Charging;
    public bool Discharging;
    public bool[] Spare1;
    public long MaxCapacity;
    public long RemainingCapacity;
    public long Rate;
    public long EstimatedTime;
    public long DefaultAlert1;
    public long DefaultAlert2;
  }

  internal class PowerManagementInterop
  {
    [DllImport("powrprof.dll", EntryPoint = "CallNtPowerInformation")]
    public static extern int LastSleepOrWakeTime(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out long lpOutputBuffer,
            int nOutputBufferSize);

    [DllImport("powrprof.dll", EntryPoint = "CallNtPowerInformation")]
    public static extern int BatteryState(
        int InformationLevel,
        IntPtr lpInputBuffer,
        int nInputBufferSize,
        out SYSTEM_BATTERY_STATE lpOutputBuffer,
        int nOutputBufferSize);

    [DllImport("powrprof.dll", EntryPoint = "CallNtPowerInformation")]
    public static extern int PowerInformation(
        int InformationLevel,
        IntPtr lpInputBuffer,
        int nInputBufferSize,
        out SYSTEM_POWER_INFORMATION lpOutputBuffer,
        int nOutputBufferSize);

    [DllImport("powrprof.dll", EntryPoint = "CallNtPowerInformation")]
    public static extern int ToggleHibernationFile(
        int InformationLevel,
        bool reserveHibernationFile,
        int nInputBufferSize,
        IntPtr lpOutputBuffer,
        int nOutputBufferSize);

    [DllImport("powrprof.dll")]
    public static extern bool SetSuspendState(
        bool Hibernate,
        bool ForceCritical,
        bool DisableWakeEvent);
  }
}
