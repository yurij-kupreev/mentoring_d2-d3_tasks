using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLog;
using PostSharp.Aspects;

namespace MentoringUnit4_WindowsServices.Code_rewriting
{
    [Serializable]
    public class CachePostSharpAspect : OnMethodBoundaryAspect
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnEntry(MethodExecutionArgs args)
        {
            var methodArguments = new StringBuilder();

            foreach (var argument in args.Arguments) {
                methodArguments.AppendLine(this.SerializeArggumentValue(argument));
            }

            Logger.Info($"Method {args.Method.Name} has been invoked with parameters: {methodArguments}");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Logger.Info($"Method {args.Method.Name} has been completed. Return value {this.SerializeArggumentValue(args.ReturnValue)}");
        }

        string SerializeArggumentValue(object argument)
        {
            try {
                return JsonConvert.SerializeObject(argument);
            } catch {
                return "Not serializable";
            }
        }
    }
}