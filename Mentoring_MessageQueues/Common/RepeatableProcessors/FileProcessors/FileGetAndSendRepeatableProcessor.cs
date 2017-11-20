using System;
using System.Threading;
using Common.Messaging;
using Common.Models;
using Common.Repositories;
using NLog;

namespace Common.RepeatableProcessors.FileProcessors
{
  public class FileGetAndSendRepeatableProcessor : IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IObjectRepository _sourceObjectRepository;
    private readonly IMessenger _destiantionMessenger;

    private readonly ProcessorStatus _processorStatus;

    public FileGetAndSendRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IMessenger destiantionMessenger)
    {
      WorkStopped = workStopped;
      _sourceObjectRepository = sourceObjectRepository;
      _destiantionMessenger = destiantionMessenger;

      _processorStatus = new ProcessorStatus()
      {
        SourceName = this.GetType().Name,
        ProcessorStartTime = DateTime.Now
      };
    }

    public void RepeatableProcess()
    {
      foreach (var objectName in _sourceObjectRepository.EnumerateObjects()) {
        if (WorkStopped.WaitOne(0)) break;

        this.ProcessFile(objectName);
      }
    }

    private void ProcessFile(string objectName)
    {
      Logger.Info($"Start processing object: {objectName}");

      Send(objectName);

      Logger.Info($"Ended processing object: {objectName}");
      _processorStatus.ProcessedObjects.Add(objectName);
    }

    private void Send(string objectName)
    {

      using (var contentStream = _sourceObjectRepository.OpenObjectStream(objectName)) {
        _destiantionMessenger.Send(new CustomMessage { Label = objectName, Body = contentStream });
      }

      _sourceObjectRepository.DeleteObject(objectName);
    }

    ProcessorStatus IRepeatableProcessor.GetProcessorStatus()
    {
      return this._processorStatus;
    }
  }
}