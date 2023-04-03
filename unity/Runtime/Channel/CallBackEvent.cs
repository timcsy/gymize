using System;
using WebSocketSharp;

namespace PAIA.Gymize
{
    public delegate void CallBack();
    public delegate void ErrorCallBack(ErrorEventArgs e);
	public delegate void ObjectCallBack(object e);
    public delegate void MessageCallBack(Content content);
    public delegate void RequestCallBack(Request request);

    public interface ICallbackEvent {}

    public class ObjectCallBackEvent : ICallbackEvent
    {
        public event ObjectCallBack Event;

        public ObjectCallBackEvent() {}
        public void Invoke(object e)
        {
            Event?.Invoke(e);
        }
    }
        
    public class MessageCallBackEvent : ICallbackEvent
    {
        public event MessageCallBack Event;
        public bool Empty
        {
            get { return Event == null; }
        }

        public MessageCallBackEvent() {}
        public void Invoke(Content content)
        {
            Event?.Invoke(content);
        }
    }
    
    public class RequestCallBackEvent : ICallbackEvent
    {
        public event RequestCallBack Event;

        public RequestCallBackEvent() {}
        public void Invoke(Request request)
        {
            Event?.Invoke(request);
        }
    }
}