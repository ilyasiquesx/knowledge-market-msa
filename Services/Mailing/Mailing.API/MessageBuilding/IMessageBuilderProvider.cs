namespace Mailing.API.MessageBuilding;

public interface IMessageBuilderProvider
{
    public IMessageBuilder GetByType(string type);
}

public class DefaultMessageBuilderProvider : IMessageBuilderProvider
{
    private readonly List<IMessageBuilder> _builders = new();

    public DefaultMessageBuilderProvider()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes()).Where(t =>
                typeof(IMessageBuilder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        var builders = types.Select(s => Activator.CreateInstance(s) as IMessageBuilder);
        _builders.AddRange(builders);
    }

    public IMessageBuilder GetByType(string type)
    {
        return _builders.FirstOrDefault(b => b.Type == type);
    }
}