using Autofac;
using DoJob.Jobs;
using Hangfire;

namespace ConsoleApp1
{

    /// <summary>
    /// 模块化注入，默认注入类型
    /// </summary>
    public class DefaultRegisterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //.InstancePerDependency();//每次请求服务（依赖实例）都会返回一个新实例（默认选项）

            #region Hangfire相关类型注入，Console类型的项目必须注入，Web项目不需要注入
            builder.RegisterType<BackgroundJobClient>().As<IBackgroundJobClient>();
            builder.RegisterType<GetNewsJob>(); 
            #endregion
        }
    }
}