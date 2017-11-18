using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.Models;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors
{
  public class ImagesConversionAndSendRepeatableProcessor: IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IObjectRepository _sourceObjectRepository;
    private readonly IMessenger _destiantionMessenger;

    private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
    private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

    public ImagesConversionAndSendRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IMessenger destiantionMessenger)
    {
      WorkStopped = workStopped;
      _sourceObjectRepository = sourceObjectRepository;
      _destiantionMessenger = destiantionMessenger;
    }

    public void RepeatableProcess()
    {
      var wasEndImage = false;
      var imageObjectNames = new List<string>();

      foreach (var objectName in _sourceObjectRepository.EnumerateObjects()) {
        if (WorkStopped.WaitOne(0)) {
          if (wasEndImage) TrySendDocuments(3, imageObjectNames);
          return;
        }

        if (IsImage(objectName) && TryToOpen(objectName, 3)) {
          wasEndImage = wasEndImage | IsEndImage(objectName);
          imageObjectNames.Add(objectName);
        }
      }

      if (wasEndImage) {
        TrySendDocuments(3, imageObjectNames);
      }
    }

    private bool IsImage(string fileName)
    {
      var regex = new Regex(ImageFileNamePattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }


    //The file should end with "End" substring. "True" result should run pdf save process.
    private bool IsEndImage(string fileName)
    {
      var regex = new Regex(EndImageFileNamePattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }

    private void TrySendDocuments(int tryCount, IEnumerable<string> imageObjectNames)
    {
      Logger.Info($"Start images sending: {string.Join(", ", imageObjectNames)}");
      for (var i = 0; i < tryCount; i++) {
        try {
          foreach (var imageObjectName in imageObjectNames)
          {
            var stream = _sourceObjectRepository.OpenObjectStream(imageObjectName);

            using (stream)
            {
              _destiantionMessenger.Send(new CustomMessage{Label = imageObjectName, Body = stream});
            }
          }

          Logger.Info("Ended image sending.");
          return;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }

    }

    private bool TryToOpen(string objectName, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try {
          var objectStream = _sourceObjectRepository.OpenObjectStream(objectName);
          objectStream.Close();

          return true;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }

      return false;
    }
  }
}