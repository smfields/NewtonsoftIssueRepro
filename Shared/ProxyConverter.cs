using Castle.DynamicProxy;
using Newtonsoft.Json.Converters;

namespace Shared;

public class ProxyConverter<T> : CustomCreationConverter<T>
{
    private static readonly ProxyGenerator Generator = new ();

    public override T Create(Type objectType)
    {
        var proxy = Generator.CreateInterfaceProxyWithoutTarget(typeof(T), new ProxyInterceptor<T>());
        return (T) proxy;
    }
}