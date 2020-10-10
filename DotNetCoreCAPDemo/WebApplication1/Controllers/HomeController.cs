using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static string eventData = "";

        private readonly ICapPublisher _capBus;
        public HomeController(
            ILogger<HomeController> logger, 
            ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
            _logger = logger;
        }

        public async Task<IActionResult> Index(HomeViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SendMsg))
            {
                var msg = DateTime.Now;
                //await _capBus.PublishAsync("xxx.services.show.time", msg);
                await _capBus.PublishAsync(GlobalConsts.AddUserActionLogEvent, model.SendMsg);
            }

            model.EventData = eventData;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async  Task<IActionResult> SendMsg(HomeViewModel model)
        {
            var msg = DateTime.Now;
            await _capBus.PublishAsync("xxx.services.show.time", msg);
            model.EventData = eventData;
            return View("Home/Index",model);
        }

        [NonAction]
        [CapSubscribe("xxx.services.show.time")]
        public void Subscriber(DateTime p)
        {
            //Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
            eventData ="触发事件接收到消息："+ p.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [NonAction]
        [CapSubscribe(GlobalConsts.AddUserActionLogEvent)]
        public void AddUserActionLogEventHandler(string msg)
        {
            //Console.WriteLine($@"{DateTime.Now} Subscriber invoked, Info: {p}");
            eventData ="触发事件接收到消息："+ msg;
        }

    }
}
