using System;

namespace SpeechToText.Interfaces
{
    public interface ISpeechToText
    {
        void StartRecording(Action<string> callback);
        void StopRecording();
    }
}
