using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using MetaProgramming.Samples;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaProgramming {
    [SimpleJob(RuntimeMoniker.Net90, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net481)]      // windows only
    public class MyBeanchMarkDxo {
        class StringModel {
            public string StringValue { get; set; }

            public string IntValue { get; set; }

            public string DoubleValue { get; set; }

            public string DatetimeValue { get; set; }
        }

        class TypeModel {
            public string StringValue { get; set; }

            public int IntValue { get; set; }

            public double DoubleValue { get; set; }

            public DateTime DatetimeValue { get; set; }
        }

        class NullableTypeModel {
            public string StringValue { get; set; }

            public int? IntValue { get; set; }

            public double? DoubleValue { get; set; }

            public DateTime? DatetimeValue { get; set; }
        }

        [GlobalSetup(Target = nameof(Refrection))]
        public void GlobalSetupRefrection() {
            ReflectionDxo.CreateObject<NullableTypeModel>(new StringModel() {
                StringValue = "abcdefg",
                IntValue = "123456",
                DoubleValue = "123.456",
                DatetimeValue = "2025/07/18 18:15:00"
            });
        }

        [GlobalSetup(Target = nameof(ExpressionTree))]
        public void GlobalSetupExpressionTree() {
            ExpressionTreeDxo.CreateObject<StringModel, NullableTypeModel>(new StringModel() {
                StringValue = "abcdefg",
                IntValue = "123456",
                DoubleValue = "123.456",
                DatetimeValue = "2025/07/18 18:15:00"
            });
        }

        [Benchmark]
        public void Simple() {
            var rnd = new Random();
            var src = new StringModel() {
                StringValue = Guid.NewGuid().ToString(),
                IntValue = rnd.Next().ToString("#####"),
                DoubleValue = rnd.NextDouble().ToString(),
                DatetimeValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };

            var dest = new NullableTypeModel();
            dest.StringValue = src.StringValue;
            dest.IntValue = int.Parse(src.IntValue);
            dest.DoubleValue = double.Parse(src.DoubleValue);
            dest.DatetimeValue = DateTime.Parse(src.DatetimeValue);
        }

        [Benchmark]
        public void Refrection() {
            var rnd = new Random();
            ReflectionDxo.CreateObject<NullableTypeModel>(new StringModel() {
                StringValue = Guid.NewGuid().ToString(),
                IntValue = rnd.Next().ToString("#####"),
                DoubleValue = rnd.NextDouble().ToString(),
                DatetimeValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }

        [Benchmark]
        public void ExpressionTree() {
            var rnd = new Random();
            ExpressionTreeDxo.CreateObject<StringModel, NullableTypeModel>(new StringModel() {
                StringValue = Guid.NewGuid().ToString(),
                IntValue = rnd.Next().ToString("#####"),
                DoubleValue = rnd.NextDouble().ToString(),
                DatetimeValue = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            });
        }
    }
}
