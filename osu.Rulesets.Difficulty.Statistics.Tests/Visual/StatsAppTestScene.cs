using osu.Framework.Testing;

namespace osu.Rulesets.Difficulty.Statistics.Tests.Visual;

[ExcludeFromDynamicCompile]
public abstract class StatsAppTestScene : TestScene
{
    protected override ITestSceneTestRunner CreateRunner() => new StatsAppTestSceneRunner();

    private class StatsAppTestSceneRunner : StatsAppBase, ITestSceneTestRunner
    {
        private TestSceneTestRunner.TestRunner runner = null!;

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            Add(runner = new TestSceneTestRunner.TestRunner());
        }

        public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
    }
}
