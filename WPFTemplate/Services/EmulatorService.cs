using System.Net.Http;
using System.Text.Json;

namespace WPFTemplate.Services
{
	public static class EmulatorService
	{
		private static readonly HttpClient HttpClient = new HttpClient();

		public static async Task<string> FetchDataAsync(string url)
		{
			try
			{
				HttpClient.Timeout = TimeSpan.FromSeconds(5);
				
				var response = await HttpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();
				
				var rawJson = await response.Content.ReadAsStringAsync();
				
				var parsedJson = JsonSerializer.Deserialize<JsonElement>(rawJson);
				var options = new JsonSerializerOptions();
				options.WriteIndented = true;
				return JsonSerializer.Serialize(parsedJson, options);
			}
			catch (HttpRequestException)
			{
				return "Ошибка сети: Эмулятор недоступен или указан неверный адрес.";
			}
			catch (TaskCanceledException)
			{
				return "Ошибка: Превышено время ожидания. Эмулятор не отвечает.";
			}
			catch (JsonException)
			{
				return "Ошибка парсинга: Получены данные, но это не корректный JSON.";
			}
			catch (Exception ex)
			{
				return $"Неизвестная ошибка: {ex.Message}";
			}
		}
	}
}