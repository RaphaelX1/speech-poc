using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

var stopRecognition = new TaskCompletionSource<int>();
var recognizer = CriarSpeechRecognizer();
RegistrarEventos(recognizer, stopRecognition);

await recognizer.StartContinuousRecognitionAsync();

Task.WaitAny(stopRecognition.Task);
static SpeechRecognizer CriarSpeechRecognizer() 
{
    var config = SpeechConfig.FromSubscription("76f2d8f3976743afad8d0ea1f31c89e0", "eastus");
    config.SpeechRecognitionLanguage = "pt-BR";
    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
    var speechRecognizer = new SpeechRecognizer(config, audioConfig);
    return speechRecognizer;
}

static void RegistrarEventos(SpeechRecognizer speechRecognizer, TaskCompletionSource<int> taskCompletionSource)
{
    speechRecognizer.Recognizing += (s, e) =>
    {
        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
    };

    speechRecognizer.Recognized += (s, e) =>
    {
        if (e.Result.Reason == ResultReason.RecognizedSpeech)
        {
            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
        }
        else if (e.Result.Reason == ResultReason.NoMatch)
        {
            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
        }
    };

    speechRecognizer.Canceled += (s, e) =>
    {
        Console.WriteLine($"CANCELED: Reason={e.Reason}");

        if (e.Reason == CancellationReason.Error)
        {
            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
            Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
        }

        taskCompletionSource.SetResult(0);
    };

    speechRecognizer.SessionStopped += (s, e) =>
    {
        Console.WriteLine("\n    Session stopped event.");
        taskCompletionSource.SetResult(0);
    };
};

