using System.Diagnostics;

namespace WpfProductionLineApp
{
    public class CameraScratch
    {



        public static Process StartWorking()
        {
            string pythonInterpreter = @"C:\Software\Anaconda3\envs\Pytorch\python.exe";
            string virtualEnvPath = @"D:\PythonProject\yoloV5_RobotScratch";
            string envPath = @"C:\Software\Anaconda3\envs\Pytorch\Lib\site-packages";

            // 设置Python程序的路径和命令行参数

            string pythonScript = @"D:\PythonProject\yoloV5_RobotScratch\main.py";
            string commandLineArgs = string.Format("\"{0}\"", pythonScript);
            // 创建新的进程并设置启动信息

            Process process = new Process();
            process.StartInfo.FileName = pythonInterpreter;
            process.StartInfo.Arguments = commandLineArgs;
            process.StartInfo.WorkingDirectory = virtualEnvPath;
            process.StartInfo.EnvironmentVariables["PYTHONPATH"] = envPath;
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;
            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.CreateNoWindow = true;

            // 启动进程并等待完成
            process.Start();
            //process.WaitForExit();
            return process;
            // 检查进程退出状态
            //if (process.ExitCode == 0)
            //{
            //    Console.WriteLine("Python program executed successfully.");
            //}
            //else
            //{
            //    Console.WriteLine("Python program execution failed with exit code: " + process.ExitCode);
            //}
            //process.Kill();
            //process.Close();

        }
    }

}
