﻿using BenchmarkDotNet.Running;

namespace MetaProgramming {
    public class Program {
        public static void Main(string[] args) {
            var summary = BenchmarkRunner.Run<MyBeanchMark>();
        }
    }
}