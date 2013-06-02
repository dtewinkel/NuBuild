using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using JetBrains.Annotations;

namespace Tasks.UnitTests
{
   public class LogInterceptor : ILogger
   {
      private IEventSource _eventSource;

      public List<BuildEventArgs> BuildEvents { get; private set; }

      public bool LogToConsole { get; set; }

      public LogInterceptor()
      {
         BuildEvents = new List<BuildEventArgs>();
         Verbosity = LoggerVerbosity.Minimal;
         LogToConsole = true;
      }

      public void ClearEvents()
      {
         BuildEvents.Clear();
      }




      public void Initialize(IEventSource eventSource)
      {
         ClearEvents();

         _eventSource = eventSource;

         _eventSource.AnyEventRaised += EventSourceOnAnyEventRaised;
         _eventSource.CustomEventRaised += EventSourceOnCustomEventRaised;
         _eventSource.BuildFinished += EventSourceOnBuildFinished;
         _eventSource.BuildStarted += EventSourceOnBuildStarted;
         _eventSource.ErrorRaised += EventSourceOnErrorRaised;
         _eventSource.MessageRaised += EventSourceOnMessageRaised;
         _eventSource.ProjectFinished += EventSourceOnProjectFinished;
         _eventSource.ProjectStarted += EventSourceOnProjectStarted;
         _eventSource.StatusEventRaised += EventSourceOnStatusEventRaised;
         _eventSource.TargetFinished += EventSourceOnTargetFinished;
         _eventSource.TargetStarted += EventSourceOnTargetStarted;
         _eventSource.TaskFinished += EventSourceOnTaskFinished;
         _eventSource.TaskStarted += EventSourceOnTaskStarted;
         _eventSource.WarningRaised += EventSourceOnWarningRaised;
      }


      private void EventSourceOnTargetStarted(object sender, TargetStartedEventArgs targetStartedEventArgs)
      {
         LogEvent(LoggerVerbosity.Detailed, "Target Start", targetStartedEventArgs);
      }


      private void EventSourceOnTargetFinished(object sender, TargetFinishedEventArgs targetFinishedEventArgs)
      {
         LogEvent(LoggerVerbosity.Detailed, "Target Finish", targetFinishedEventArgs);
      }


      private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
      {
         LogEvent(LoggerVerbosity.Detailed, "Project Start", projectStartedEventArgs);
      }


      private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
      {
         LogEvent(LoggerVerbosity.Detailed, "Project Finish", projectFinishedEventArgs);
      }


      private void EventSourceOnBuildStarted(object sender, BuildStartedEventArgs buildStartedEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Build Start", buildStartedEventArgs);
      }


      private void EventSourceOnBuildFinished(object sender, BuildFinishedEventArgs buildFinishedEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Build Finish", buildFinishedEventArgs);
      }


      private void EventSourceOnTaskFinished(object sender, TaskFinishedEventArgs taskFinishedEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Task Finish", taskFinishedEventArgs);
      }


      private void EventSourceOnTaskStarted(object sender, TaskStartedEventArgs taskStartedEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Task Start", taskStartedEventArgs);
      }


      private void EventSourceOnCustomEventRaised(object sender, CustomBuildEventArgs customBuildEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Custom", customBuildEventArgs);
      }


      private void EventSourceOnStatusEventRaised(object sender, BuildStatusEventArgs buildStatusEventArgs)
      {
         LogEvent(LoggerVerbosity.Diagnostic, "Status", buildStatusEventArgs);
      }


      private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs buildWarningEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Warning", buildWarningEventArgs, "({0}, {1}) ",
                      buildWarningEventArgs.LineNumber, buildWarningEventArgs.ColumnNumber);
      }


      private void EventSourceOnMessageRaised(object sender, BuildMessageEventArgs buildMessageEventArgs)
      {
         LogEvent(LoggerVerbosity.Normal, "Message", buildMessageEventArgs);
      }


      private void EventSourceOnAnyEventRaised(object sender, BuildEventArgs buildEventArgs)
      {
         LogEvent(LoggerVerbosity.Detailed, "Event", buildEventArgs);
      }


      private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs buildErrorEventArgs)
      {
         LogEvent(LoggerVerbosity.Quiet, "Error", buildErrorEventArgs, "({0}, {1}) ",
                      buildErrorEventArgs.LineNumber, buildErrorEventArgs.ColumnNumber);
      }


      [StringFormatMethod("message")]
      private void LogEvent(LoggerVerbosity verbosity, [NotNull] string eventName, [NotNull] BuildEventArgs buildevent, [NotNull] string message = "", params object[] parameters)
      {
         BuildEvents.Add(buildevent);

         if (LogToConsole && verbosity <= Verbosity)
         {
            string formatedMessage = string.Format(message, parameters);
            Console.WriteLine("{0}: [{1}] {2}{3}", eventName.PadRight(14), buildevent.Timestamp, formatedMessage, buildevent.Message);
         }
      }


      public void Shutdown()
      {
         _eventSource.AnyEventRaised -= EventSourceOnAnyEventRaised;
         _eventSource.CustomEventRaised -= EventSourceOnCustomEventRaised;
         _eventSource.BuildFinished -= EventSourceOnBuildFinished;
         _eventSource.BuildStarted -= EventSourceOnBuildStarted;
         _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
         _eventSource.MessageRaised -= EventSourceOnMessageRaised;
         _eventSource.ProjectFinished -= EventSourceOnProjectFinished;
         _eventSource.ProjectStarted -= EventSourceOnProjectStarted;
         _eventSource.StatusEventRaised -= EventSourceOnStatusEventRaised;
         _eventSource.TargetFinished -= EventSourceOnTargetFinished;
         _eventSource.TargetStarted -= EventSourceOnTargetStarted;
         _eventSource.TaskFinished -= EventSourceOnTaskFinished;
         _eventSource.TaskStarted -= EventSourceOnTaskStarted;
         _eventSource.WarningRaised -= EventSourceOnWarningRaised;
      }

      public LoggerVerbosity Verbosity { get; set; }
      public string Parameters { get; set; }
   }
}