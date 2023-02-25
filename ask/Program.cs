using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

if (args.Length > 0) {
    using (var httpClient = new HttpClient()) {
        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.openai.com/v1/completions")) {
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer YOUR_API_KEY");

            request.Content = new StringContent("{\n  \"model\": \"text-davinci-001\",\n  \"prompt\": \"" + args[0] + "\",\n  \"max_tokens\": 100,\n  \"temperature\": 1\n}", Encoding.UTF8); // From documentation, model is text-davinci-003, max_tokens is 7, temperature is 0

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);

            string responseString = await response.Content.ReadAsStringAsync();

            try {
                var s = JsonConvert.DeserializeObject<dynamic>(responseString);

                string guess = GuessCommand(s!.choices[0].text);

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"---> API response is: {guess}");

                Console.ResetColor();
            }
            catch (Exception ex) {
                Console.WriteLine($"---> Could not deserialize the JSON: {ex.Message}");
            }

            // Console.WriteLine(responseString);
        }
    }
}
else {
    Console.WriteLine("---> You need to provide some input");
}

static string GuessCommand(string input) {
    Console.WriteLine("---> GPT-3 API returned text:");

    Console.ForegroundColor = ConsoleColor.Yellow;

    Console.WriteLine(input);

    var lastIndexOf = input.LastIndexOf("\n");

    string s = input.Substring(lastIndexOf + 1);

    Console.ResetColor();

    TextCopy.ClipboardService.SetText(s);

    return s;
}
