using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    public class ImpinjHttpServer
    {
        private Server server;

        public ImpinjHttpServer()
        {
            this.server = new Server(new Listener(), new RouteRepository());
        }

        public void Start (string port = "4200")
        {
            AutoResetEvent keepAlive = new AutoResetEvent(false);
            this.server.StartAsync(port);
            keepAlive.WaitOne();
        }

        public void Get(string endpoint, Action<Request, Response> handler)
        {
            this.server.RouteRepository.Get.Add(endpoint, handler);
        }

        public void Post(string endpoint, Action<Request, Response> handler)
        {
            this.server.RouteRepository.Post.Add(endpoint, handler);
        }

        public void Put(string endpoint, Action<Request, Response> handler)
        {
            this.server.RouteRepository.Put.Add(endpoint, handler);
        }

        public void Delete(string endpoint, Action<Request, Response> handler)
        {
            this.server.RouteRepository.Delete.Add(endpoint, handler);
        }
    }
}
