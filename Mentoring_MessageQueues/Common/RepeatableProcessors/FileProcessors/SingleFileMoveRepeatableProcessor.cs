using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Common.Repositories;
using NLog;

namespace Common.RepeatableProcessors.FileProcessors
{
  public class SingleFileMoveRepeatableProcessor : IRepeatableProcessor
  {
    private object _locker = new object();

    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IObjectRepository _sourceObjectRepository;
    private readonly IObjectRepository _destiantionObjectRepository;

    private readonly ProcessorStatus _processorStatus;

    public SingleFileMoveRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IObjectRepository destiantionObjectRepository)
    {
      WorkStopped = workStopped;
      _sourceObjectRepository = sourceObjectRepository;
      _destiantionObjectRepository = destiantionObjectRepository;

      _processorStatus = new ProcessorStatus
      {
        SourceName = this.GetType().Name,
        ProcessorStartTime = DateTime.Now
      };
    }

    public void RepeatableProcess()
    {
      var tasks = new List<Task>();

      foreach (var objectName in _sourceObjectRepository.EnumerateObjects()) {
        if (WorkStopped.WaitOne(0)) break;

        tasks.Add(this.ProcessFile(objectName));
      }

      Task.WaitAll(tasks.ToArray());
    }

    private async Task ProcessFile(string objectName)
    {
      Logger.Info($"Start processing object: {objectName}");

      await MoveAsync(objectName);

      Logger.Info($"Ended processing object: {objectName}");

      lock (_locker)
      {
        _processorStatus.ProcessedObjects.Add(objectName);
      }
    }

    private async Task MoveAsync(string objectName)
    {
      using (var contentStream = _sourceObjectRepository.OpenObjectStream(objectName)) {
        await _destiantionObjectRepository.SaveObjectAsync(objectName, contentStream);
      }

      _sourceObjectRepository.DeleteObject(objectName);
    }

    public ProcessorStatus GetProcessorStatus()
    {
      return _processorStatus;
    }
  }
}
