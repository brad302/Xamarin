using System;
using AVFoundation;
using Speech;
using SpeechToText.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpeechToText.iOS.DependencyServices.SpeechToText))]
namespace SpeechToText.iOS.DependencyServices
{
    public class SpeechToText : ISpeechToText
    {
        AVAudioEngine _audioEngine;
        SFSpeechRecognizer _speechRecognizer;
        SFSpeechAudioBufferRecognitionRequest _speechRecognitionRequest;
        SFSpeechRecognitionTask _speechRecognitionTask;

        public SpeechToText()
        {
            _audioEngine = new AVAudioEngine();
            _speechRecognizer = new SFSpeechRecognizer();
            _speechRecognitionRequest = new SFSpeechAudioBufferRecognitionRequest();
            _speechRecognitionTask = new SFSpeechRecognitionTask();

            SFSpeechRecognizer.RequestAuthorization((SFSpeechRecognizerAuthorizationStatus status) =>
            {
                // We're going to assume that you've selected to authorise the request, otherwise,
                // you're wasting your own time.
            });
        }

        public void StartRecording(Action<string> callback)
        {
            var audioSession = AVAudioSession.SharedInstance();

            var error = audioSession.SetCategory(AVAudioSessionCategory.Record);
            audioSession.SetMode(AVAudioSession.ModeMeasurement, out error);
            error = audioSession.SetActive(true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);

            _speechRecognitionRequest = new SFSpeechAudioBufferRecognitionRequest();

            var inputNode = _audioEngine.InputNode;
            var recordingFormat = inputNode.GetBusOutputFormat(0);

            inputNode.InstallTapOnBus(0, 1024, recordingFormat, (buffer, when) =>
            {
                _speechRecognitionRequest?.Append(buffer);
            });

            _audioEngine.Prepare();
            _audioEngine.StartAndReturnError(out error);

            _speechRecognitionTask = _speechRecognizer.GetRecognitionTask(_speechRecognitionRequest, (result, recognitionError) =>
            {
                if (result != null)
                {
                    var transcribedText = result.BestTranscription.FormattedString;
                    callback?.Invoke(transcribedText);
                }
            });
        }

        public void StopRecording()
        {
            try
            {
                _audioEngine.Stop();
                _audioEngine.InputNode.RemoveTapOnBus(0);
                _speechRecognitionTask?.Cancel();
                _speechRecognitionRequest.EndAudio();
                _speechRecognitionRequest = null;
                _speechRecognitionTask = null;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
