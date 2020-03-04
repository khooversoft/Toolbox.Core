using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class FizzBuzz
    {
        private static List<Func<int, string>> _evaulations = new List<Func<int, string>>
        {
            x => x == 0 ? x.ToString() : null,
            x => x % 3 == 0 && x % 5 == 0 ? "FizzBuzz" : null,
            x => x % 3 == 0 ? "Fizz" : null,
            x => x % 5 == 0 ? "Buzz" : null,
            x => x.ToString(),
        };

        public Task<string> Evaluate(int value)
        {
            string result = _evaulations
                .Select(x => x.Invoke(value))
                .SkipWhile(x => x == null)
                .First();

            return Task.FromResult(result);
        }

        [Benchmark]
        public void FizzBuzz100()
        {
            const int max = 100;
            for(int i = 0; i < max; i++)
            {
                Evaluate(i);
            }
        }
    }
}
