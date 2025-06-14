# MetaProgramming

メタプログラミング入門サンプル兼ベンチマーク

## ベンチマーク動かし方

### 前提条件

-   [dotnet 9.0 の SDK](https://dotnet.microsoft.com/ja-jp/download/dotnet/9.0)をインストール

### 以下コマンドを実行

Windows

```powershell
cd .\MetaProgramming
dotnet run -c Release --framework net9.0
```

Mac

```bash
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
-   ExpressoinTree
    -   動的にメソッドを生成できる。そこそこ手軽に使え、それなりに速度もでる
-   IL 生成
    -   dotnet が生成する中間言語である IL を直接生成する
    -   メソッドに加え、クラス、アセンブリ自体（DLL）を動的に生成することも可能

#### 番外編

-   dyanmic を使うという手もある

    -   内部的には「式木による動的コード生成」＋「生成したコードのキャッシュ」を行っている

-   イベントハンドラをフックするときなどは delegate を使う
    -   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/how-to-hook-up-a-delegate-using-reflection

### 参考サイト

-   https://ufcpp.net/study/csharp/misc_dynamic.html
-   https://neue.cc/2011/04/20_317.html
-   https://neue.cc/2014/01/27_446.html
-   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/reflection
-   https://learn.microsoft.com/ja-jp/dotnet/fundamentals/reflection/emitting-dynamic-methods-and-assemblies
