using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class APIRequest : MonoBehaviour
{

    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }
    public string groqApiKey;
    public string elevenlabsApiKey;

    private List<object> conversationHistory = new List<object>
    {
        new { role = "system", content = "Eres un asistente muy inteligente. " +
            "Es importante que hagas respuestas cortas ya que el usuario va a  " +
            "escucharte utilizando TTS y es un poco caro con IElevenLabs " }
    };

    private async Task<string> MakeRequestAPILlama(string prompt)
    {
        conversationHistory.Add(new { role = "user", content = prompt });

        using (HttpClient client = new HttpClient())
        {
            string url = "https://api.groq.com/openai/v1/chat/completions";

            var jsonData = new
            {
                messages = conversationHistory,
                model = "llama-3.3-70b-specdec",
                temperature = 1,
                max_tokens = 1024,
                top_p = 1,
                stream = false,
                stop = (string)null
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);

            // Configurar el contenido de la solicitud
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");


            // Agregar el encabezado de autorización
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqApiKey}");

            try
            {
                // Enviar la petición POST
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Asegurarse de que la respuesta fue exitosa
                response.EnsureSuccessStatusCode();

                // Leer el contenido de la respuesta
                string responseBody = await response.Content.ReadAsStringAsync();


                // Procesar la respuesta
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                // Manejar errores de la petición
                Debug.LogError("Error en la petición: " + e.Message);
                return null;
            }
        }
    }

    private async Task<string> MakeRequestAPIWhisper(string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://api.groq.com/openai/v1/audio/transcriptions";

            var formdata = new MultipartFormDataContent();

            byte[] fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
            formdata.Add(fileContent, "file", "audio.wav");

            formdata.Add(new StringContent("whisper-large-v3-turbo"), "model");
            formdata.Add(new StringContent("0"), "temperature");
            formdata.Add(new StringContent("json"), "response_format");
            formdata.Add(new StringContent("es"), "language");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqApiKey}");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, formdata);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log(responseBody);

                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Error en la petición: " + e.Message);
                return null;
            }
        }
    }

    private async Task MakeAPIRequestElevenLabs(string prompt)
    {

        using (var client = new HttpClient()) 
        {
            string url = "https://api.elevenlabs.io/v1/text-to-speech/7ilYbYb99yBZGMUUKSaf/stream?output_format=pcm_16000";

            var jsonData = new
            {
                model_id = "eleven_multilingual_v2",
                text = prompt,
                voice_settings = new
                {
                    use_speaker_boost = true,
                    stability = 0.5,
                    similarity_boost = 1
                }
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);

            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");


            client.DefaultRequestHeaders.Add("xi-api-key", elevenlabsApiKey);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    MemoryStream memoryStream = new MemoryStream();

                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);

                        // Procesa el fragmento de audio
                        ProcessAudioFragment(buffer, bytesRead);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Error en la petición: " + e.Message);
            }
        }
        
    }

    private void ProcessAudioFragment(byte[] buffer, int bytesRead)
    {
        int sampleCount = bytesRead / 2;
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(buffer, i * 2);
            samples[i] = sample / 32768.0f;
        }

        int channels = 1; // Asumimos mono
        int sampleRate = 16000; // Frecuencia de muestreo de 16 kHz

        AudioClip clip = AudioClip.Create("audio", samples.Length, channels, sampleRate, false);
        clip.SetData(samples, 0);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;

        audioSource.Play();
    }


    public async Task incializarAPITexto()
    {
        string jsonResponseWhisper = await MakeRequestAPIWhisper(Application.persistentDataPath + "/audio.wav");

        JObject jsonObject = JObject.Parse(jsonResponseWhisper);

        JToken respuestaWhisper = jsonObject["text"];

        string salidaWhisper = "";

        if (respuestaWhisper != null)
        {
            salidaWhisper = respuestaWhisper.ToString();
            this.promptWhisper = salidaWhisper;
        }
        else
        {
            Console.WriteLine("La propiedad 'text' no existe en el JSON.");
        }

        string jsonResponseLlama = await MakeRequestAPILlama(salidaWhisper);
        conversationHistory.Add(new { role = "assistant", content = jsonResponseLlama });

        JObject json = JObject.Parse(jsonResponseLlama);
        JArray jarray = (JArray)json["choices"];
        JObject firstChoice = (JObject)jarray[0];
        JObject message = (JObject)firstChoice["message"];
        string content = message["content"].ToString();

        this.promptLLama = content;
    }
}
