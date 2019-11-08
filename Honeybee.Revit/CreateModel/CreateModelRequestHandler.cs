using System;
using System.Threading;
using Autodesk.Revit.UI;
using NLog;

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelRequestHandler : IExternalEventHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CreateModelRequest Request { get; } = new CreateModelRequest();

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                    {
                        return;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                throw;
            }
        }

        public string GetName()
        {
            return "Create Model External Event";
        }
    }

    public enum RequestId
    {
        None
    }

    public class CreateModelRequest
    {
        private int _request = (int)RequestId.None;

        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref _request, (int)RequestId.None);
        }

        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref _request, (int)request);
        }
    }
}
