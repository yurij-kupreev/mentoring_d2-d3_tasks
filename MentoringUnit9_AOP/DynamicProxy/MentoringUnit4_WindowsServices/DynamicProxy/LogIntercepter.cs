using System.Text;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using NLog;

namespace MentoringUnit4_WindowsServices.DynamicProxy
{
    public class LogInterceptor : IInterceptor
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Intercept(IInvocation invocation)
        {
            var methodArguments = new StringBuilder();

            foreach (var argument in invocation.Arguments)
            {
                methodArguments.AppendLine(this.SerializeArggumentValue(argument));
            }

            Logger.Info($"Method {invocation.Method.Name} has been invoked with parameters: {methodArguments}");

            invocation.Proceed();

            Logger.Info($"Method {invocation.Method.Name} has been completed. Return value {this.SerializeArggumentValue(invocation.ReturnValue)}");
        }

        string SerializeArggumentValue(object argument)
        {
            try
            {
                return JsonConvert.SerializeObject(argument);
            }
            catch
            {
                return "Not serializable";
            }
        }
    }
}