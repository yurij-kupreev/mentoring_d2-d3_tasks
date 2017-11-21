using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Messaging;
using Common.Models;
using Common.Repositories;
using NLog;

namespace Common.RepeatableProcessors.ImageSetProcessors
{
  //public class ImageSetGetAndSendRepeatableProcessor : IRepeatableProcessor
  //{
  //  public WaitHandle WorkStopped { get; set; }

  //  private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

  //  private readonly IObjectRepository _sourceObjectRepository;
  //  private readonly IMessenger _destiantionMessenger;

  //  private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
  //  private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

  //  private readonly ProcessorStatus _processorStatus;

  //  public ImageSetGetAndSendRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IMessenger destiantionMessenger)
  //  {
  //    WorkStopped = workStopped;
  //    _sourceObjectRepository = sourceObjectRepository;
  //    _destiantionMessenger = destiantionMessenger;

  //    _processorStatus = new ProcessorStatus
  //    {
  //      SourceName = this.GetType().Name,
  //      ProcessorStartTime = DateTime.Now
  //    };
  //  }

  //  public void RepeatableProcess()
  //  {
  //    var wasEndImage = false;
  //    var imageObjectNames = new List<string>();

  //    foreach (var objectName in _sourceObjectRepository.EnumerateObjects()) {
  //      if (WorkStopped.WaitOne(0)) {
  //        if (wasEndImage) SendDocuments(imageObjectNames);
  //        return;
  //      }

  //      if (IsImage(objectName)) {
  //        wasEndImage = wasEndImage | IsEndImage(objectName);
  //        imageObjectNames.Add(objectName);
  //      }
  //    }

  //    if (wasEndImage) {
  //      SendDocuments(imageObjectNames);
  //    }
  //  }

  //  private bool IsImage(string fileName)
  //  {
  //    var regex = new Regex(ImageFileNamePattern, RegexOptions.IgnoreCase);

  //    return regex.IsMatch(fileName);
  //  }


  //  //The file should end with "End" substring. "True" result should run pdf save process.
  //  private bool IsEndImage(string fileName)
  //  {
  //    var regex = new Regex(EndImageFileNamePattern, RegexOptions.IgnoreCase);

  //    return regex.IsMatch(fileName);
  //  }

  //  private void SendDocuments(IEnumerable<string> imageObjectNames)
  //  {
  //    Logger.Info($"Start images sending: {string.Join(", ", imageObjectNames)}");

  //    foreach (var imageObjectName in imageObjectNames) {
  //      var stream = _sourceObjectRepository.OpenObjectStream(imageObjectName);

  //      using (stream) {
  //        _destiantionMessenger.Send(new CustomMessage { Label = imageObjectName, Body = stream });
  //      }
  //    }

  //    Logger.Info("Ended image sending.");

  //    _processorStatus.ProcessedObjects.AddRange(imageObjectNames);
  //  }

  //  public ProcessorStatus GetProcessorStatus()
  //  {
  //    return _processorStatus;
  //  }
  //}
}