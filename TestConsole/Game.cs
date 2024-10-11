using System;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using NUnitLite;

namespace TestConsole
{
    public class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Console.WriteLine("TestConsole is running!");

            // Run the existing assembly loading and type checking
            CheckAssemblies();

            Console.WriteLine("\nRunning tests...");

            // Run the NUnit tests
            return new AutoRun().Execute(args);
        }

        static void CheckAssemblies()
        {
            try
            {
                string? executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
                string? basePath = Path.GetDirectoryName(executingAssemblyPath);

                if (basePath == null)
                {
                    Console.WriteLine("Unable to determine base path.");
                    return;
                }

                string gameStatePath = Path.Combine(basePath, "GameState.dll");
                string gameInputPath = Path.Combine(basePath, "GameInput.dll");

                Console.WriteLine($"Looking for GameState.dll at: {gameStatePath}");
                Console.WriteLine($"Looking for GameInput.dll at: {gameInputPath}");

                Assembly gameStateAssembly = Assembly.LoadFile(gameStatePath);
                Assembly gameInputAssembly = Assembly.LoadFile(gameInputPath);

                Console.WriteLine($"GameState assembly loaded: {gameStateAssembly != null}");
                Console.WriteLine($"GameInput assembly loaded: {gameInputAssembly != null}");

                if (gameStateAssembly != null)
                {
                    Console.WriteLine("\nTypes in GameState:");
                    foreach (var type in gameStateAssembly.GetTypes())
                    {
                        Console.WriteLine($"  {type.FullName}");
                    }
                }

                if (gameInputAssembly != null)
                {
                    Console.WriteLine("\nTypes in GameInput:");
                    foreach (var type in gameInputAssembly.GetTypes())
                    {
                        Console.WriteLine($"  {type.FullName}");
                    }
                }

                Type? gameManagerType = gameStateAssembly?.GetType("GameState.GameManager");
                Console.WriteLine($"\nGameManager type found: {gameManagerType != null}");
                if (gameManagerType == null && gameStateAssembly != null)
                {
                    Console.WriteLine("Attempting to find GameManager type with Assembly.GetType:");
                    foreach (var type in gameStateAssembly.GetTypes())
                    {
                        if (type.Name == "GameManager")
                        {
                            Console.WriteLine($"Found type with name GameManager: {type.FullName}");
                            gameManagerType = type;
                            break;
                        }
                    }
                }

                if (gameManagerType != null)
                {
                    var instance = Activator.CreateInstance(gameManagerType, 42);
                    Console.WriteLine($"Created instance of GameManager: {instance != null}");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"ReflectionTypeLoadException: {ex.Message}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    if (loaderException != null)
                        Console.WriteLine($"Loader Exception: {loaderException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
