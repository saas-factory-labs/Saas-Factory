using AppBlueprint.SharedKernel;
using AutoBogus;
using Newtonsoft.Json;
using Assembly = System.Reflection.Assembly;

namespace AppBlueprint.Infrastructure.DatabaseContexts.FakeData.FakeDataGeneration;

public class FakeDataGenerator
{
    private readonly Dictionary<Type, object> _fakerConfigurations = new();

    public FakeDataGenerator()
    {
        IEnumerable<Type>? entityTypes = Assembly.GetAssembly(typeof(IEntity)).GetTypes()
            .Where(t => t.Namespace == "Shared.Models");

        foreach (Type? type in entityTypes)
        {
            Type? autoFakerType = typeof(AutoFaker<>).MakeGenericType(type);
            object? autoFaker = Activator.CreateInstance(autoFakerType);

            _fakerConfigurations[type] = autoFaker;
        }
    }

    public T GenerateFakeData<T>() where T : class
    {
        if (_fakerConfigurations.TryGetValue(typeof(T), out object? configuration))
        {
            object? autoFaker = configuration.GetType().GetMethod("Generate").Invoke(configuration, null);
            return (T)autoFaker;
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
