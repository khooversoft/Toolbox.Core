using FluentAssertions;
using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Toolbox.Standard.Test.Tools
{
    public class StateManagerTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public async Task SimpleStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().Be(true);
            workPlan.StateItems.Count.Should().Be(1);
        }

        [Fact]
        public async Task SimpleNotifyTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeTrue();
            workPlan.StateItems.Count.Should().Be(1);
        }

        [Fact]
        public async Task MultipleStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Add(new StateItemSuccess())
                .Add(new StateItemSuccess())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeTrue();
            workPlan.StateItems.Count.Should().Be(3);
        }

        [Fact]
        public async Task FailureStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(1);
        }

        [Fact]
        public async Task Failure2StateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Add(new StateItemSuccess())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(2);
        }

        [Fact]
        public async Task SuccessAndFailureStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Add(new StateItemFailure())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(2);
        }

        [Fact]
        public async Task SuccessTestStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Build();

            bool result = await workPlan.Test(_workContext);
            result.Should().BeTrue();
            workPlan.StateItems.Count.Should().Be(1);
        }

        [Fact]
        public async Task SuccessTest2StateStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Add(new StateItemAlreadyPresent())
                .Build();

            bool result = await workPlan.Set(_workContext);
            result.Should().BeTrue();
            workPlan.StateItems.Count.Should().Be(2);
        }

        [Fact]
        public async Task SuccessTestFailedStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Build();

            bool result = await workPlan.Test(_workContext);
            result.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(1);
        }

        [Fact]
        public async Task SuccessTestFailed2StateStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Add(new StateItemFailure())
                .Build();

            bool result = await workPlan.Test(_workContext);
            result.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(2);
        }

        private class StateItemSuccess : StateItemBase
        {
            public StateItemSuccess()
                : base("Success", false, false, true)
            {
            }
        }

        private class StateItemFailure : StateItemBase
        {
            public StateItemFailure()
                : base("Failure", false, false, false)
            {
            }
        }

        private class StateItemAlreadyPresent : StateItemBase
        {
            public StateItemAlreadyPresent()
                : base("Present", false, true, false)
            {
            }
        }

        private class StateItemBase : IStateItem
        {
            private bool _resultFromTest;
            private bool _resultFromSet;

            public StateItemBase(string name, bool ignoreError, bool resultFromTest, bool resultFromSet)
            {
                Name = name;
                IgnoreError = ignoreError;
                _resultFromTest = resultFromTest;
                _resultFromSet = resultFromSet;
            }

            public string Name { get; }

            public bool IgnoreError { get; }

            public Task<bool> Set(IWorkContext context)
            {
                return Task.FromResult(_resultFromSet);
            }

            public Task<bool> Test(IWorkContext context)
            {
                return Task.FromResult(_resultFromTest);
            }
        }
    }
}
