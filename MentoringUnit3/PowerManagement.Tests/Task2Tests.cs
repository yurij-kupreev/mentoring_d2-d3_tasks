using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerManagement.Tests
{
  [TestClass]
  public class Task2Tests
  {
    private readonly IPowerManagementClient _powerManagementClient;

    public Task2Tests()
    {
      _powerManagementClient = new PowerManagementClient();
    }

    [TestMethod]
    public void SetSetSuspendStateTest()
    {
      _powerManagementClient.SetSuspendState(true, false, false);
    }
  }
}