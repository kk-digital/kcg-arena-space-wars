using GameState;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameInput
{
    public interface IKeyboardState
    {
        bool IsKeyDown(Keys key);
    }

    public class InputManager
    {
        private List<InputCommand> inputBuffer = new List<InputCommand>();
        private int currentFrame = 0;
        private bool isRecording = false;
        private bool isReplaying = false;
        private const string DefaultFileName = "recorded_inputs.json";

        public void Update(IKeyboardState keyboardState, GameManager gameManager)
        {
            if (isReplaying)
            {
                ReplayFrame(gameManager);
                return;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                ExecuteCommand(new AccelerateCommand(5f), gameManager);
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ExecuteCommand(new RotateCommand(-0.1f), gameManager);
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                ExecuteCommand(new RotateCommand(0.1f), gameManager);
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                ExecuteCommand(new FireCommand(), gameManager);
            }

            currentFrame++;
        }

        private void ExecuteCommand(InputCommand command, GameManager gameManager)
        {
            command.Frame = currentFrame;
            command.Execute(gameManager);
            if (isRecording) inputBuffer.Add(command);
        }

        public void StartRecording()
        {
            isRecording = true;
            inputBuffer.Clear();
            currentFrame = 0;
            Console.WriteLine("Recording started.");
        }

        public void StopRecording()
        {
            isRecording = false;
            SaveInputBufferToJson(DefaultFileName);
            Console.WriteLine("Recording stopped and saved.");
        }

        public void StartReplay(GameManager gameManager)
        {
            LoadInputBufferFromJson(DefaultFileName);
            isReplaying = true;
            currentFrame = 0;
            Console.WriteLine("Replay started.");
        }

        public void StopReplay()
        {
            isReplaying = false;
            Console.WriteLine("Replay stopped.");
        }

        public bool IsRecording() => isRecording;
        public bool IsReplaying() => isReplaying;
        public bool HasReplayEnded() => !isReplaying && currentFrame >= inputBuffer.Count;
        public int GetCurrentFrame() => currentFrame;

        private void ReplayFrame(GameManager gameManager)
        {
            var commands = inputBuffer.FindAll(cmd => cmd.Frame == currentFrame);
            foreach (var command in commands)
            {
                command.Execute(gameManager);
            }
            currentFrame++;

            if (currentFrame >= inputBuffer.Count)
            {
                StopReplay();
            }
        }

        public void SaveInputBufferToJson(string filename)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = new List<JsonConverter> { new InputCommandConverter() },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(inputBuffer, settings);
                File.WriteAllText(filename, json);
                Console.WriteLine($"Input buffer saved to {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving input buffer: {e.Message}");
            }
        }

        public void LoadInputBufferFromJson(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    string json = File.ReadAllText(filename);
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Converters = new List<JsonConverter> { new InputCommandConverter() },
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    inputBuffer = JsonConvert.DeserializeObject<List<InputCommand>>(json, settings) ?? new List<InputCommand>();
                    Console.WriteLine($"Input buffer loaded from {filename}");
                }
                else
                {
                    Console.WriteLine($"File not found: {filename}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading input buffer: {e.Message}");
            }
        }

        public void ClearInputBuffer()
        {
            inputBuffer.Clear();
            currentFrame = 0;
            Console.WriteLine("Input buffer cleared.");
        }
    }
}
