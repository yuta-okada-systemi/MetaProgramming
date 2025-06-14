using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace MetaProgramming {
    [SimpleJob(RuntimeMoniker.Net90, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net481)]      // windows only
    public class MyBeanchMark {
        private readonly SimpleImpl _simpleImpl = new SimpleImpl();
        private readonly RefrectionImpl _refrectionImpl = new RefrectionImpl();
        private readonly ExpressionTreeImpl _expressionTreeImpl = new ExpressionTreeImpl();
        private readonly ILImpl _iLImpl = new ILImpl();

        [GlobalSetup(Target = nameof(Refrection))]
        public void GlobalSetupRefrection() {
            _refrectionImpl.Setup();
        }

        [GlobalSetup(Target = nameof(ExpressionTree))]
        public void GlobalSetupExpressionTree() {
            _expressionTreeImpl.Setup();
        }

        [GlobalSetup(Target = nameof(IL))]
        public void GlobalSetupIL() {
            _iLImpl.Setup();
        }

        [Benchmark]
        public int Simple() => _simpleImpl.Invoke();

        [Benchmark]
        public int Refrection() => _refrectionImpl.Invoke();

        [Benchmark]
        public int ExpressionTree() => _expressionTreeImpl.Invoke();

        [Benchmark]
        public int IL() => _iLImpl.Invoke();
    }
}

public class Point {
    public int X { get; set; }
    public int Y { get; set; }
    public int Value { get; set; } = 10;

    public int Sum(int value) => X + Y + value;
}

public class SimpleImpl {
    public int Invoke() {
        var instance = new Point();
        instance.X = 100;
        instance.Y = 200;
        return instance.Sum(instance.Value);
    }
}

public class RefrectionImpl {
    private ConstructorInfo ctor;
    private PropertyInfo xProperty;
    private PropertyInfo yProperty;
    private PropertyInfo valueProperty;
    private MethodInfo sumMethod;

    public void Setup() {
        var pointType = typeof(Point);
        var ctor = pointType.GetConstructor(Type.EmptyTypes);
        var map = pointType.GetProperties().ToDictionary(p => p.Name);
        this.ctor = ctor;
        xProperty = map["X"];
        yProperty = map["Y"];
        valueProperty = map["Value"];

        sumMethod = pointType.GetMethod("Sum");
    }

    public int Invoke() {
        var instance = (Point)ctor.Invoke(Array.Empty<object>());

        xProperty.SetValue(instance, 100);
        yProperty.SetValue(instance, 200);
        return (int)sumMethod.Invoke(instance, new object[] { valueProperty.GetValue(instance) });
    }

}

public class ExpressionTreeImpl {
    private Func<int> _invokeSum;

    public void Setup() {
        var pointType = typeof(Point);
        var ctor = pointType.GetConstructor(Type.EmptyTypes);
        var map = pointType.GetProperties().ToDictionary(p => p.Name);
        var sumMethod = pointType.GetMethod("Sum");

        // Point instane;
        // instance = new Point();
        var instance = Expression.Variable(pointType, "instance");
        var createInstance = Expression.Assign(instance, Expression.New(ctor));

        // instance.X = 100;
        var assignX = Expression.Assign(
            Expression.Property(instance, map["X"]), Expression.Constant(100)
        );

        // instance.Y = 200;
        var assignY = Expression.Assign(
            Expression.Property(instance, map["Y"]), Expression.Constant(200)
        );

        // instance.Sum(instance.Value);
        var getValue = Expression.Property(instance, map["Value"]);
        var callSum = Expression.Call(instance, sumMethod, getValue);

        var block = Expression.Block(
            new[] { instance },
            createInstance,
            assignX,
            assignY,
            callSum
        );

        _invokeSum = Expression.Lambda<Func<int>>(block).Compile();
    }

    public int Invoke() {
        return _invokeSum.Invoke();
    }

}


public class ILImpl {
    private Func<int> _invokeSum;

    public void Setup() {
        var pointType = typeof(Point);
        var ctor = pointType.GetConstructor(Type.EmptyTypes);
        var map = pointType.GetProperties().ToDictionary(p => p.Name);
        var sumMethod = pointType.GetMethod("Sum");

        var method = new DynamicMethod(
            "InvokePointSum",
            typeof(int),
            Type.EmptyTypes,
            typeof(Point).Module
        );
        var il = method.GetILGenerator();

        // ローカル変数: Point instance
        il.DeclareLocal(pointType);

        // instance = new Point();
        il.Emit(OpCodes.Newobj, ctor);
        il.Emit(OpCodes.Stloc_0);

        // instance.X = 100;
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldc_I4, 100);
        il.Emit(OpCodes.Call, map["X"].GetSetMethod());

        // instance.Y = 200;
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldc_I4, 200);
        il.Emit(OpCodes.Call, map["Y"].GetSetMethod());

        // instance.Sum(instance.Value)
        il.Emit(OpCodes.Ldloc_0); // instance
        il.Emit(OpCodes.Ldloc_0); // instance
        il.Emit(OpCodes.Call, map["Value"].GetGetMethod()); // instance.Value
        il.Emit(OpCodes.Call, sumMethod); // instance.Sum(instance.Value)

        il.Emit(OpCodes.Ret);

        _invokeSum = (Func<int>)method.CreateDelegate(typeof(Func<int>));
    }

    public int Invoke() {
        return _invokeSum.Invoke();
    }

}

