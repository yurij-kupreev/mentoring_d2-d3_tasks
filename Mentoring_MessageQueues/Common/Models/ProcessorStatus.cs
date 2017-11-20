using System;
using System.Collections.Generic;

namespace Common.Models
{
  [Serializable]
  public class ProcessorStatus
  {
    public string SourceName { get; set; }
    public List<string> ProcessedObjects { get; set; }
    public DateTime ProcessorStartTime { get; set; }

    public ProcessorStatus()
    {
      ProcessedObjects = new List<string>();
    }

    public override string ToString()
    {
      return
        $"Processor name: {SourceName} \nUp time: {DateTime.Now - ProcessorStartTime} \nProcessedObjects: {string.Join(" ", ProcessedObjects)}";
    }
  }
}