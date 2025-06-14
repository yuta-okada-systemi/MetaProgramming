using BenchmarkDotNet.Running;

namespace MetaProgramming {
    public class Program {
        public static void Main(string[] args) {
            // //var impl = new ILImpl();
            // //var impl = new ExpressionTreeImpl();
            // var impl = new RefrectionImpl();
            // impl.Setup();
            // System.Diagnostics.Debug.WriteLine(impl.Invoke());

            var summary = BenchmarkRunner.Run<MyBeanchMark>();
        }
    }
}