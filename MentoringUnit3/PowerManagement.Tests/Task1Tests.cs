using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerManagement.Tests
{
  [TestClass]
  public class Task1Tests
  {
    private readonly IPowerManagementClient _powerManagementClient;

    public Task1Tests()
    {
      _powerManagementClient = new PowerManagementClient();
    }

    [TestMethod]
    public void LastSleepTimeTest()
    {
      var lastSleepTime = _powerManagementClient.LastSleepTime();

      Console.WriteLine(lastSleepTime);
    }

    [TestMethod]
    public void LastWakeTimeTest()
    {
      var lastWakeTime = _powerManagementClient.LastWakeTime();

      Console.WriteLine(lastWakeTime);
    }

    [TestMethod]
    public void PowerInformationTest()
    {
      var powerInformation = _powerManagementClient.PowerInformation();

      Console.WriteLine(powerInformation.Idleness);
    }

    [TestMethod]
    public void BatteryStateTest()
    {
      var batteryState = _powerManagementClient.BatteryState();

      Console.WriteLine(batteryState.BatteryPresent);
    }
  }
}
