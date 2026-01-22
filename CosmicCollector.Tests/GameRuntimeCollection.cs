using Xunit;

namespace CosmicCollector.Tests;

/// <summary>
/// Коллекция для тестов с синглтоном runtime.
/// </summary>
[CollectionDefinition("GameRuntime", DisableParallelization = true)]
public sealed class GameRuntimeCollection
{
}
