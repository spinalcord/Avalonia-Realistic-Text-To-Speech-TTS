using ReactiveUI;
using System.IO;
using System.Net.Http;
using System;
using System.Reactive;
using NAudio.Wave;

namespace AvaloniaRealisticTextToSpeech.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ReactiveCommand<string,Unit> SpeechCommand { get; }

    public MainViewModel() 
    {
        SpeechCommand = ReactiveCommand.Create<string>(Speech);
    }


    public async void SpeechLogic(string input)
    {

        // Hello umändern
        string url = "http://localhost:5002/api/tts?text=" + input;

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                // Send the GET request to the URL and await the response
                HttpResponseMessage response = await httpClient.GetAsync(url);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response as a byte array
                    byte[] responseData = await response.Content.ReadAsByteArrayAsync();

                    // You can work with the data in the memory (responseData) here

                    // Play the WAV file directly from memory
                    //using (MemoryStream memoryStream = new MemoryStream(responseData))
                    //{
                    //  SoundPlayer player = new SoundPlayer(memoryStream);
                    // player.PlaySync(); // Or player.Play() if you want to start the playback asynchronously
                    //}

                    using (MemoryStream memoryStream = new MemoryStream(responseData))
                    {
                        using (WaveStream waveStream = new WaveFileReader(memoryStream))
                        {
                            using (WaveOutEvent waveOut = new WaveOutEvent())
                            {
                                waveOut.Init(waveStream);
                                waveOut.Play(); 
                                while (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    System.Threading.Thread.Sleep(100);
                                }
                            }
                        }
                    }

                    Console.WriteLine("Data played successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: The request was not successful. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending the request: {ex.Message}");
            }
        }

    }


    public void Speech(string parameter)
    {
        SpeechLogic(parameter);
    }

}
