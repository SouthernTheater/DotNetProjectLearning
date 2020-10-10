using System;

namespace Common
{
    public class GlobalConsts
    {
        //public const string CapExchangeName = "cap.default.router";

        public const string CapExchangeName = "cap.testapp.router";

        /// <summary>
        /// Subscriber default group name. kafka--&gt;group name. rabbitmq --&gt; queue name.
        /// SubscriptionClientName
        /// 默认后缀为应用程序名+v1
        /// </summary>
        public const string CapDefaultGroup = "cap.queue.testapp.v1";

        public const string AddUserActionLogEvent = "AddUserActionLogEvent";

        /// <summary>
        /// 事件总线重试次数
        /// </summary>
        public const int EventBusRetryCount = 2;

    }
}
