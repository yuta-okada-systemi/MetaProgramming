# MetaProgramming

メタプログラミング入門サンプル兼ベンチマーク

## ベンチマーク動かし方

### 前提条件

-   [dotnet 9.0 の SDK](https://dotnet.microsoft.com/ja-jp/download/dotnet/9.0)をインストール

### 以下コマンドを実行

Windows

```powershell
dotnet restore
cd .\MetaProgramming
dotnet run -c Release --framework net9.0
```

Mac

-   `MetaProgramming/MyBeanchMark.cs`
    -   Net481 の行をコメントアウト

```Csharp
    [SimpleJob(RuntimeMoniker.Net90, baseline: true)]
    //[SimpleJob(RuntimeMoniker.Net481)]      // windows only
    public class MyBeanchMark {
```

-   `MetaProgramming\MetaProgramming.csproj`
    -   `TargetFramework`のコメントアウトを外す
    -   `TargetFrameworks`の行をコメントアウト

```xml
    <TargetFramework>net9.0</TargetFramework>
    <!-- <TargetFrameworks>net481;net9.0</TargetFrameworks> -->
```

```bash
dotnet restore
cd ./MetaProgramming
dotnet run -c Release --framework net9.0
```

## メタプログラミングとは？

-   プログラムが自分自身を操作・生成・解析する高度なプログラミング技術
-   プログラムのコードを動的に生成したり、変更したり、解析したりすることができる
-   知っておけば、共通処理やライブラリを作ったりするときに役に立つ（かも）

## C#におけるメタプログラミング方法

### 大きく以下の 3 種類

-   Reflection
    -   手軽だが遅い
    -   リフレクション情報自体を取得する処理は重い処理。キャッシュ化が必要
-   ExpressoinTree
    -   動的にメソッドを生成できる。そこそこ手軽に使え、それなりに速度もでる
    -   `Compile`は重い処理なので、生成したメソッドのキャッシュ化が必要
-   IL 生成
    -   dotnet が生成する中間言語である IL を直接生成する
    -   メソッドに加え、クラス、アセンブリ自体（DLL）を動的に生成することも可能
    -   `IL生成`は重い処理なので、生成メソッド、クラスのキャッシュ化が必要
    -   注意事項としてネイティブ AOT ではサポートされていない
        -   https://learn.microsoft.com/ja-jp/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8

#### 番外編

-   dyanmic を使うという手もある

    -   内部的には「式木による動的コード生成」＋「生成したコードのキャッシュ」を行っている

-   イベントハンドラをフックするときなどは delegate が使える
    -   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/how-to-hook-up-a-delegate-using-reflection

### 参考サイト

-   https://ufcpp.net/study/csharp/misc_dynamic.html
-   https://neue.cc/2011/04/20_317.html
-   https://neue.cc/2014/01/27_446.html
-   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/reflection
-   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/emitting-dynamic-methods-and-assemblies
