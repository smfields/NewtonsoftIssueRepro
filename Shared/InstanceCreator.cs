using Newtonsoft.Json;

namespace Shared;

public static class InstanceCreator
{
    public static IMyInterface CreateInstance()
    {
        return JsonConvert.DeserializeObject<IMyInterface>("{\"MyProperty\": \"Hello\"}", new ProxyConverter<IMyInterface>())!;
    }
}