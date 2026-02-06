using AppBlueprint.SharedKernel;
using AutoBogus;
using Newtonsoft.Json;
using Assembly = System.Reflection.Assembly;

namespace AppBlueprint.SeedTest.FakeDataGeneration;

internal sealed class FakeDataGenerator
{
    private readonly Dictionary<Type, object> _fakerConfigurations = new();

    public FakeDataGenerator()
    {
        Assembly? assembly = Assembly.GetAssembly(typeof(IEntity));
        if (assembly == null) return;

        IEnumerable<Type> entityTypes = assembly.GetTypes()
            .Where(t => t.Namespace == "Shared.Models");

        foreach (Type? type in entityTypes)
        {
            Type? autoFakerType = typeof(AutoFaker<>).MakeGenericType(type);
            object? autoFaker = Activator.CreateInstance(autoFakerType);

            if (autoFaker is not null)
            {
                _fakerConfigurations[type] = autoFaker;
            }
        }
    }

    public T GenerateFakeData<T>() where T : class
    {
        if (_fakerConfigurations.TryGetValue(typeof(T), out object? configuration))
        {
            object? autoFaker = configuration.GetType().GetMethod("Generate")?.Invoke(configuration, null);
            if (autoFaker is T result)
            {
                return result;
            }
        }

        return new AutoFaker<T>().Generate();
    }

    public void GenerateAndSaveFakeData<T>(string filePath) where T : class
    {
        T? fakeData = GenerateFakeData<T>();
        string? json = JsonConvert.SerializeObject(fakeData, Formatting.Indented);

        File.WriteAllText(filePath, json);
    }
}
