using System.Threading.Tasks;
using GaleriaArteFrontend.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace GaleriaArteFrontend.Services
{
    public class ObrasService
    {
        // Aquí se implementarán los métodos para consumir el API Gateway
        public async Task<List<Obra>> GetObrasAsync()
        {
            await AddJwtHeaderAsync();
            // Obtiene las obras del artista autenticado
            var response = await _httpClient.GetAsync("obra/mis-obras");
            if (!response.IsSuccessStatusCode)
                return new List<Obra>();
            var obras = await response.Content.ReadFromJsonAsync<List<Obra>>();
            return obras ?? new List<Obra>();
        }

        public async Task<Obra?> GetObraByIdAsync(int id)
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"obra/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Obra>();
        }

        public async Task<bool> CrearObraAsync(object obra, IBrowserFile archivo)
        {
            await AddJwtHeaderAsync();
            // Asume que obra es del tipo ObraCrearModel
            var model = obra as dynamic;
            if (model == null || archivo == null)
                return false;

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Titulo), "Titulo");
            if (!string.IsNullOrEmpty(model.Descripcion))
                content.Add(new StringContent(model.Descripcion), "Descripcion");
            content.Add(new StringContent(model.Precio.ToString()), "Precio");
            // El nickname del artista se debe obtener del usuario autenticado (por ejemplo, desde un AuthService o JWT). Aquí se usa un valor de ejemplo.
            content.Add(new StringContent("demo-nickname"), "ArtistaNickname");

            var stream = archivo.OpenReadStream(10 * 1024 * 1024); // 10MB max
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(archivo.ContentType);
            content.Add(fileContent, "Archivo", archivo.Name);

            var response = await _httpClient.PostAsync("api/obra/crear", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarObraAsync(int id, object obra)
        {
            await AddJwtHeaderAsync();
            // Asume que obra es del tipo ObraCrearModel o similar
            var model = obra as dynamic;
            if (model == null)
                return false;
            var updateDto = new
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                Precio = model.Precio
            };
            var response = await _httpClient.PutAsJsonAsync($"obra/{id}", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> OcultarObraAsync(int id)
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.PatchAsync($"obra/{id}/ocultar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActivarObraAsync(int id)
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.PatchAsync($"obra/{id}/activar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarObraAsync(int id)
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.DeleteAsync($"obra/{id}");
            return response.IsSuccessStatusCode;
        }

        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public ObrasService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task AddJwtHeaderAsync()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5Mzc5NjczYy01MjAyLTQ2MjQtYTQ3ZS00NzEzMjQ2YTkyOWEiLCJlbWFpbCI6Im1pbHRvbi5hLnBhc3Rvci5mQGdtYWlsLmNvbSIsIm5pY2tuYW1lIjoibWlsdG9uIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiNjJkMmI2MWYtZDkyYi00NGQ4LWFkYTgtOWQ1ZGFjZTdlNmJjIiwiZXhwIjoxNzUzMDU2NzU0LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDIiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDIifQ.Tommj-_hnVha-_6j56LhehuY4l0iSEitsKyXMcmONNk";
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
