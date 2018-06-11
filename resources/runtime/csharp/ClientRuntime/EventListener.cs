
namespace Microsoft.Rest.ClientRuntime
{

    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GetEventData=System.Func<EventData>;

    public interface IValidates
    {
        Task Validate(Microsoft.Rest.ClientRuntime.IEventListener listener);
    }

    /// <summary>
    /// The IEventListener Interface defines the communication mechanism for Signaling events during a remote call.
    /// </summary>
    /// <remarks>
    /// The interface is designed to be as minimal as possible, allow for quick peeking of the event type (<c>id</c>) 
    /// and the cancellation status and provides a delegate for retrieving the event details themselves.
    /// </remarks>
    public interface IEventListener {
          Task Signal(string id, CancellationToken token, GetEventData createMessage);
          CancellationToken Token { get; }
    }

#if OLD
    public interface IEventListener
    {
        Task Signal(string id, GetEventData createMessage);
        bool IsCancellationRequested { get; }
        CancellationToken Token { get; }
        void CancelAfter(int millisecondsDelay);
        void CancelAfter(TimeSpan delay);
        void Cancel();
    }
#endif
    public static class IEventListenerExtensions
    {
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  Func<EventData> createMessage) => instance.Signal(id, token, createMessage);
        public static Task Signal(this IEventListener instance, string id,CancellationToken token) => instance.Signal(id, token,() => new EventData { });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText) => instance.Signal(id, token, () => new EventData { Message = messageText });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText, HttpRequestMessage request) => instance.Signal(id, token, () => new EventData { Message = messageText, RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText, HttpResponseMessage response) => instance.Signal(id,token, () => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText, double magnitude) => instance.Signal(id,token, () => new EventData { Message = messageText, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText, double magnitude, HttpRequestMessage request) => instance.Signal(id, token,() => new EventData { Message = messageText, RequestMessage = request, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  string messageText, double magnitude, HttpResponseMessage response) => instance.Signal(id, token,() => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  HttpRequestMessage request) => instance.Signal(id,token, () => new EventData { RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  HttpRequestMessage request, HttpResponseMessage response) => instance.Signal(id, token,() => new EventData { RequestMessage = request, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  HttpResponseMessage response) => instance.Signal(id, token,() => new EventData { RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, CancellationToken token,  EventData message) => instance.Signal(id, token,() => message);

      public static Task Signal(this IEventListener instance, string id,  Func<EventData> createMessage) => instance.Signal(id, instance.Token, createMessage);
        public static Task Signal(this IEventListener instance, string id) => instance.Signal(id, instance.Token,() => new EventData { });
        public static Task Signal(this IEventListener instance, string id,  string messageText) => instance.Signal(id, instance.Token, () => new EventData { Message = messageText });
        public static Task Signal(this IEventListener instance, string id,  string messageText, HttpRequestMessage request) => instance.Signal(id, instance.Token, () => new EventData { Message = messageText, RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id,  string messageText, HttpResponseMessage response) => instance.Signal(id,instance.Token, () => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id,  string messageText, double magnitude) => instance.Signal(id,instance.Token, () => new EventData { Message = messageText, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id,  string messageText, double magnitude, HttpRequestMessage request) => instance.Signal(id, instance.Token,() => new EventData { Message = messageText, RequestMessage = request, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id,  string messageText, double magnitude, HttpResponseMessage response) => instance.Signal(id, instance.Token,() => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id,  HttpRequestMessage request) => instance.Signal(id,instance.Token, () => new EventData { RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id,  HttpRequestMessage request, HttpResponseMessage response) => instance.Signal(id, instance.Token,() => new EventData { RequestMessage = request, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id,  HttpResponseMessage response) => instance.Signal(id, instance.Token,() => new EventData { RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id,  EventData message) => instance.Signal(id, instance.Token,() => message);


        public static async Task AssertNotNull(this IEventListener instance, string parameterName, object value)
        {
            if (value == null)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"'{parameterName}' should not be null", Parameter = parameterName });
            }
        }
        public static async Task AssertMinimumLength(this IEventListener instance, string parameterName, string value, int length)
        {
            if (value != null && value.Length < length)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"Length of '{parameterName}' is less than {length}", Parameter = parameterName });
            }
        }
        public static async Task AssertMaximumLength(this IEventListener instance, string parameterName, string value, int length)
        {
            if (value != null && value.Length > length)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"Length of '{parameterName}' is greater than {length}", Parameter = parameterName });
            }
        }

        public static async Task AssertRegEx(this IEventListener instance, string parameterName, string value, string regularExpression)
        {
            if (value != null && !System.Text.RegularExpressions.Regex.Match(value, regularExpression).Success)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"'{parameterName}' does not validate against pattern /{regularExpression}/", Parameter = parameterName });
            }
        }
        public static async Task AssertEnum(this IEventListener instance, string parameterName, string value, params string[] values)
        {
            if (!values.Any(each => each.Equals(value)))
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"'{parameterName}' is not one of ({values.Aggregate((c, e) => $"'{e}',{c}")}", Parameter = parameterName });
            }
        }
        public static async Task AssertObjectIsValid(this IEventListener instance, string parameterName, object inst)
        {
            await (inst as Microsoft.Rest.ClientRuntime.IValidates)?.Validate(instance);
        }

        public static async Task AssertIsLessThan<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) >= 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"Value of '{parameterName}' must be less than {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsGreaterThan<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) <= 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"Value of '{parameterName}' must be greater than {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsLessThanOrEqual<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) > 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"Value of '{parameterName}' must be less than or equal to {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsGreaterThanOrEqual<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) < 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"Value of '{parameterName}' must be greater than or equal to {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, Int64? value, Int64 multiple)
        {
            if (null != value && value % multiple != 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, double? value, double multiple)
        {
            if (null != value)
            {
                var i = (Int64)(value / multiple);
                if (i != value / multiple)
                {
                    await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning,instance.Token, () => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
                }
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, decimal? value, decimal multiple)
        {
            if (null != value)
            {
                var i = (Int64)(value / multiple);
                if (i != value / multiple)
                {
                    await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, instance.Token,() => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
                }
            }
        }
    }


#if OLD
    public static class IEventListenerExtensions
    {
        public static Task Signal(this IEventListener instance, string id, Func<EventData> createMessage) => instance.Signal(id, createMessage);
        public static Task Signal(this IEventListener instance, string id) => instance.Signal(id, () => new EventData { });
        public static Task Signal(this IEventListener instance, string id, string messageText) => instance.Signal(id, () => new EventData { Message = messageText });
        public static Task Signal(this IEventListener instance, string id, string messageText, HttpRequestMessage request) => instance.Signal(id, () => new EventData { Message = messageText, RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id, string messageText, HttpResponseMessage response) => instance.Signal(id, () => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, string messageText, double magnitude) => instance.Signal(id, () => new EventData { Message = messageText, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, string messageText, double magnitude, HttpRequestMessage request) => instance.Signal(id, () => new EventData { Message = messageText, RequestMessage = request, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, string messageText, double magnitude, HttpResponseMessage response) => instance.Signal(id, () => new EventData { Message = messageText, RequestMessage = response.RequestMessage, ResponseMessage = response, Value = magnitude });
        public static Task Signal(this IEventListener instance, string id, HttpRequestMessage request) => instance.Signal(id, () => new EventData { RequestMessage = request });
        public static Task Signal(this IEventListener instance, string id, HttpRequestMessage request, HttpResponseMessage response) => instance.Signal(id, () => new EventData { RequestMessage = request, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, HttpResponseMessage response) => instance.Signal(id, () => new EventData { RequestMessage = response.RequestMessage, ResponseMessage = response });
        public static Task Signal(this IEventListener instance, string id, EventData message) => instance.Signal(id, () => message);

        public static async Task AssertNotNull(this IEventListener instance, string parameterName, object value)
        {
            if (value == null)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"'{parameterName}' should not be null", Parameter = parameterName });
            }
        }
        public static async Task AssertMinimumLength(this IEventListener instance, string parameterName, string value, int length)
        {
            if (value != null && value.Length < length)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Length of '{parameterName}' is less than {length}", Parameter = parameterName });
            }
        }
        public static async Task AssertMaximumLength(this IEventListener instance, string parameterName, string value, int length)
        {
            if (value != null && value.Length > length)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Length of '{parameterName}' is greater than {length}", Parameter = parameterName });
            }
        }

        public static async Task AssertRegEx(this IEventListener instance, string parameterName, string value, string regularExpression)
        {
            if (value != null && !System.Text.RegularExpressions.Regex.Match(value, regularExpression).Success)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"'{parameterName}' does not validate against pattern /{regularExpression}/", Parameter = parameterName });
            }
        }
        public static async Task AssertEnum(this IEventListener instance, string parameterName, string value, params string[] values)
        {
            if (!values.Any(each => each.Equals(value)))
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"'{parameterName}' is not one of ({values.Aggregate((c, e) => $"'{e}',{c}")}", Parameter = parameterName });
            }
        }
        public static async Task AssertObjectIsValid(this IEventListener instance, string parameterName, object inst)
        {
            await (inst as Microsoft.Rest.ClientRuntime.IValidates)?.Validate(instance);
        }


        public static async Task AssertIsLessThan<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) >= 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be less than {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsGreaterThan<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) <= 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be greater than {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsLessThanOrEqual<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) > 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be less than or equal to {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsGreaterThanOrEqual<T>(this IEventListener instance, string parameterName, T? value, T max) where T : struct, System.IComparable<T>
        {
            if (null != value && ((T)value).CompareTo(max) < 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be greater than or equal to {max} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, Int64? value, Int64 multiple)
        {
            if (null != value && value % multiple != 0)
            {
                await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, double? value, double multiple)
        {
            if (null != value)
            {
                var i = (Int64)(value / multiple);
                if (i != value / multiple)
                {
                    await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
                }
            }
        }
        public static async Task AssertIsMultipleOf(this IEventListener instance, string parameterName, decimal? value, decimal multiple)
        {
            if (null != value)
            {
                var i = (Int64)(value / multiple);
                if (i != value / multiple)
                {
                    await instance.Signal(Microsoft.Rest.ClientRuntime.Events.ValidationWarning, () => new EventData { Message = $"Value of '{parameterName}' must be multiple of {multiple} (value is {value})", Parameter = parameterName });
                }
            }
        }

    }
#endif     

    /// <summary>
    /// An Implementation of the IEventListener that supports subscribing to events and dispatching them
    /// (used for manually using the lowlevel interface)
    /// </summary>
    public class EventListener : CancellationTokenSource, IEnumerable<KeyValuePair<string, Event>>, IEventListener
    {
        private Dictionary<string, Event> calls = new Dictionary<string, Event>();
        public IEnumerator<KeyValuePair<string, Event>> GetEnumerator() => calls.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => calls.GetEnumerator();
        public EventListener()
        {
        }

        private Event tracer;

        public EventListener(params (string name, Event callback)[] initializer)
        {
            foreach (var each in initializer)
            {
                Add(each.name, each.callback);
            }
        }

        public void Add(string name, SynchEvent callback)
        {
            Add(name, (message) => { callback(message); return Task.CompletedTask; });
        }

        public void Add(string name, Event callback)
        {
            if (callback != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    if (calls.ContainsKey(name))
                    {
                        tracer += callback;
                    }
                    else
                    {
                        tracer = callback;
                    }
                }
                else
                {
                    if (calls.ContainsKey(name))
                    {
                        calls[name ?? ""] += callback;
                    }
                    else
                    {
                        calls[name ?? ""] = callback;
                    }
                }
            }
        }


        public async Task Signal(string id, CancellationToken token, GetEventData createMessage) {
            if (!string.IsNullOrEmpty(id) && (calls.TryGetValue(id, out Event listener) || tracer != null))
            {
                var message = createMessage();
                message.Id = id;

                await listener?.Invoke(message);
                await tracer?.Invoke(message);

                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException($"Canceled by event {id} ", this.Token);
                }
            }
        }
    }
}