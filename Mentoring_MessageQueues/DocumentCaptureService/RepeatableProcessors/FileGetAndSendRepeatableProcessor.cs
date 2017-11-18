using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.Models;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors
{
  public class FileGetAndSendRepeatableProcessor : IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IObjectRepository _sourceObjectRepository;
    private readonly IMessenger _destiantionMessenger;

    public FileGetAndSendRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IMessenger destiantionMessenger)
    {
      WorkStopped = workStopped;
      _sourceObjectRepository = sourceObjectRepository;
      _destiantionMessenger = destiantionMessenger;
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

      TryToSend(objectName, 3);

      Logger.Info($"Ended processing object: {objectName}");
    }

    private void TryToSend(string objectName, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try {
          using (var contentStream = _sourceObjectRepository.OpenObjectStream(objectName)) {
            _destiantionMessenger.Send(new CustomMessage{Label = objectName, Body = contentStream });
          }

          _sourceObjectRepository.DeleteObject(objectName);

          return;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }
    }
  }
}