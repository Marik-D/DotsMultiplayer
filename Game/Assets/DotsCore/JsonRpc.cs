using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotsCore
{
    public class JsonRpc : IDisposable
    {
        private readonly Action<string> _send;
        private readonly Dictionary<string, Func<JToken, JToken>> _handlers = new Dictionary<string, Func<JToken, JToken>>();
        private readonly Dictionary<int, TaskCompletionSource<JToken>> _requests = new Dictionary<int, TaskCompletionSource<JToken>>();
        private int _nextId = 0;

        public JsonRpc(Action<string> send)
        {
            this._send = send;
        }

        public void HandleMessageFromTransport(string msgStr)
        {
            var msg = JObject.Parse(msgStr);
            if (msg["method"] != null)
            {

                var method = msg["method"].Value<string>();

                var handler = this._handlers[method];

                var res = handler(msg["params"]);

                var response = new
                {
                    jsonrpc = "2.0",
                    id = msg["id"].Value<int>(),
                    result = res,
                };

                this._send(JsonConvert.SerializeObject(response));
            }
            else
            {
                var id = msg["id"].Value<int>();
                this._requests[id].SetResult(msg);
            }
        }

        public void Dispose()
        {
            foreach (var taskCompletionSource in this._requests)
            {
                taskCompletionSource.Value.SetCanceled();
            }
        }

        public JsonRpc Handle<TArg, TRes>(string method, Func<TArg, TRes> handler)
        {
            _handlers.Add(method, arg =>
            {
                var parsed = arg.ToObject<TArg>();
                var res = handler(parsed);
                if (res == null)
                {
                    return null;
                }
                return JToken.FromObject(res);
            });
            
            return this;
        }
        
        public JsonRpc Handle<TArg>(string method, Action<TArg> handler)
        {
            _handlers.Add(method, arg =>
            {
                var parsed = arg.ToObject<TArg>();
                handler(parsed);
                return null;
            });
            
            return this;
        }
        
        public async Task<TRes> Call<TArg, TRes>(string method, TArg arg)
        {
            var id = this._nextId++;
            
            var responseTask = new TaskCompletionSource<JToken>();
            this._requests[id] = responseTask;
            
            var request = new
            {
                jsonrpc = "2.0",
                method = method,
                id = id,
                @params = arg,
            };
            this._send(JsonConvert.SerializeObject(request));

            var response = await responseTask.Task;
            
            this._requests.Remove(id);

            if (response["error"] != null)
            {
                throw new Exception(response["error"].Value<string>());
            }

            return response["result"].ToObject<TRes>();
        }
    }
}