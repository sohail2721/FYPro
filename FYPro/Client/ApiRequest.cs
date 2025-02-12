﻿using System;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using static System.Net.WebRequestMethods;

namespace FYPro.Client
{
    public class ApiRequest
    {
        public static HttpClient http { get; set; }
        public static IJSRuntime jsr { get; set; }
        public static ISnackbar Snackbar = MudItems.Snackbar;

        public static async Task<dynamic> PostRequest<T>(string Url, Object Model) where T : class
        {
            var Create = JsonSerializer.Serialize(Model);
            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Headers.Add("Authorization", "Bearer " + await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false));
            request.Content = new StringContent(Create, System.Text.Encoding.UTF8);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await http.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return null;
            }
            if (response.IsSuccessStatusCode)
            {
                if (response.IsSuccessStatusCode)
                {
                    var v = await response.Content.ReadFromJsonAsync<T>();
                    return v;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static async Task<dynamic> PostAndGetAsInt(string Url, Object Model)
        {
            var Create = JsonSerializer.Serialize(Model);
            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Headers.Add("Authorization", "Bearer " + await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false));
            request.Content = new StringContent(Create, System.Text.Encoding.UTF8);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                if (response.IsSuccessStatusCode)
                {
                    var v = await response.Content.ReadFromJsonAsync<int>();
                    return v;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<dynamic> GetRequest<T>(string Url) where T : class
        {
            var RequestMsg = new HttpRequestMessage(HttpMethod.Get, Url);
            RequestMsg.Headers.Add("Authorization", "Bearer " + await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false));
            var response = await http.SendAsync(RequestMsg);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await jsr.InvokeVoidAsync("localStorage.removeItem", "user").ConfigureAwait(false);
                    MudItems.SnackBar("User Logged Out.", Defaults.Classes.Position.TopLeft, Severity.Error);
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                   
                    return null;
                }
                else if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<T>();
                    return list;
                }
                
                return null;
            }
            else
            {
                
                return null;
            }
        }

        public static async Task<dynamic> GetRequestAsInt(string Url)
        {
            var RequestMsg = new HttpRequestMessage(HttpMethod.Get, Url);
            RequestMsg.Headers.Add("Authorization", "Bearer " + await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false));
            var response = await http.SendAsync(RequestMsg);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await jsr.InvokeVoidAsync("localStorage.removeItem", "user").ConfigureAwait(false);
                    
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                   
                    return null;
                }
                else if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<int>();
                    return list;
                }
                
                return null;
            }
            else
            {
               
                return null;
            }
        }

        public static async Task<dynamic> GetRequestAsBool(string Url)
        {
            var RequestMsg = new HttpRequestMessage(HttpMethod.Get, Url);
            RequestMsg.Headers.Add("Authorization", "Bearer " + await jsr.InvokeAsync<string>("localStorage.getItem", "user").ConfigureAwait(false));
            var response = await http.SendAsync(RequestMsg);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await jsr.InvokeVoidAsync("localStorage.removeItem", "user").ConfigureAwait(false);
                   
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                   

                    return null;
                }
                else if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<bool>();
                    return list;
                }
               

                return null;
            }
            else
            {
               
                return null;
            }
        }
    }
}
