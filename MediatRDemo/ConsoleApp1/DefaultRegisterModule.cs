using Autofac;
using ConsoleApp1.DoService;

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
            builder.RegisterType<DoTestService>().InstancePerDependency();
        }
    }
}