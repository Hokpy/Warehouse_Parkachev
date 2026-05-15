using Xunit;

// Disable parallel test execution because all tests share static InMemoryStorage
[assembly: CollectionBehavior(DisableTestParallelization = true)]