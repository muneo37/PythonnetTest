using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using System.IO;

namespace PythonnetTest
{
    class Program
    {
        /// <summary>
        /// プロセスの環境変数PATHに、指定されたディレクトリを追加する(パスを通す)。
        /// </summary>
        /// <param name="paths">PATHに追加するディレクトリ。</param>
        public static void AddEnvPath(params string[] paths)
        {
            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
            foreach (var path in paths)
            {
                if (path.Length > 0 && !envPaths.Contains(path))
                {
                    envPaths.Insert(0, path);
                }
            }
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
        }

        /// <summary>
        /// プログラムのエントリポイント。
        /// </summary>
        /// <param name="args">コマンドライン引数。</param>
        static void Main(string[] args)
        {
            // *-------------------------------------------------------*
            // * python環境の設定
            // *-------------------------------------------------------*

            // python環境にパスを通す
            // TODO: 環境に合わせてパスを直すこと
            //var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"%userprofile%\Anaconda3\envs\pythonnet_test");
            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\ProgramData\Anaconda3");

            // pythonnetが、python本体のDLLおよび依存DLLを見つけられるようにする
            AddEnvPath(
              PYTHON_HOME,
              Path.Combine(PYTHON_HOME, @"Library\bin")
            );

            // python環境に、PYTHON_HOME(標準pythonライブラリの場所)を設定
            PythonEngine.PythonHome = PYTHON_HOME;

            // python環境に、PYTHON_PATH(モジュールファイルのデフォルトの検索パス)を設定
            PythonEngine.PythonPath = string.Join(
              Path.PathSeparator.ToString(),
              new string[] {
                  PythonEngine.PythonPath,// 元の設定を残す
                  Path.Combine(PYTHON_HOME, @"Lib\site-packages"), //pipで入れたパッケージはここに入る
                  //Path.Combine(@"C:\foo\bar\my_packages"), //自分で作った(動かしたい)pythonプログラムの置き場所も追加
                  Path.Combine(@"D:\source\repos\VisualStudio2015\PythonnetTest\PythonCode"), //自分で作った(動かしたい)pythonプログラムの置き場所も追加

              }
            );

            // 初期化 (明示的に呼ばなくても内部で自動実行されるようだが、一応呼ぶ)
            PythonEngine.Initialize();

            // *-------------------------------------------------------*
            // * pythonコードの実行
            // *-------------------------------------------------------*
            // Global Interpreter Lockを取得
            using (Py.GIL())
            {
                // モジュールの探索パスを表示 (pythonコードを直接指定して実行)
                PythonEngine.RunSimpleString(@"
                                            import sys
                                            import pprint
                                            print('module path =')
                                            pprint.pprint(sys.path)
                                            ");

                // numpyのオブジェクトを取得し、呼び出してみる
                //dynamic np = Py.Import("numpy");
                //Console.WriteLine($"np.cos(np.pi * 2) =  {np.cos(np.pi * 2)}");

                // 自作コードも叩ける
                //dynamic myMath = Py.Import("my_awesome_lib.my_math"); // "from my_awesome_lib import my_math"
                dynamic myMath = Py.Import("PythonCode"); // "from my_awesome_lib import my_math"
                dynamic calculator = myMath.Calculator(5, 7); // クラスのインスタンスを生成
                Console.WriteLine($"5 + 7 = {calculator.add()}"); // クラスのメソッド呼び出し
                Console.WriteLine($"2回目 = {calculator.add()}"); // クラスのメソッド呼び出し
                Console.WriteLine($"sum(1,2,3,4,5) = {myMath.Calculator.sum(new[] { 1, 2, 3, 4, 5 })}"); //staticメソッドも当然呼べる
                dynamic dict = myMath.GetDict(); // 辞書型を返す関数呼び出し
                Console.WriteLine(dict[3]); // 辞書からキーを指定して読み取り
                dynamic ten = myMath.tensor();
                //Console.WriteLine(ten);
            }

            // python環境を破棄
            PythonEngine.Shutdown();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
