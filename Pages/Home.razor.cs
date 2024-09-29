using System.Globalization;
using System.Text;
using CsvHelper;
using FakeUserGenerator.Models;
using FakeUserGenerator.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FakeUserGenerator.Pages
{
    public partial class Home : ComponentBase, IAsyncDisposable
    {
        private int pageNumber = 0;
        private bool isLoading = false;
        private int seed = 1; // Default seed value
        private string region = "US+English"; // Default region
        private double errorCount = 0; // Default error count
        private List<User> users = new();
        private DotNetObjectReference<Home>? dotNetObjectReference;

        [Inject]
        public IFakeUserGeneratorService UserGeneratorService { get; set; } = null!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            await RefreshUsers();
        }

        private async Task RefreshUsers()
        {
            pageNumber = 0;
            users.Clear();
            await GenerateUsers();
        }

        private async Task GenerateUsers()
        {
            isLoading = true;
            var generatedUsers = await Task.Run(() => UserGeneratorService.GenerateUsers(region, errorCount, seed, pageNumber, 20));
            users.AddRange(generatedUsers);
            isLoading = false;
        }

        private async Task RandomizeSeed()
        {
            seed = Random.Shared.Next(1, 1000000);
            await RefreshUsers();
        }

        [JSInvokable]
        public async Task LoadMoreData()
        {
            if (isLoading) return;

            isLoading = true;
            pageNumber++;

            // Generate more users asynchronously
            var newUsers = await Task.Run(() => UserGeneratorService.GenerateUsers(region, errorCount, seed, pageNumber, 20));
            users.AddRange(newUsers);
            isLoading = false;

            // Optional: Trigger UI update manually if necessary
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeInfiniteScroll", dotNetObjectReference);
            }
        }

        private async Task ExportToCSV()
        {
            var csvBuilder = new StringBuilder();
            using (var writer = new StringWriter(csvBuilder))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (users.Count > 0)
                {
                    await csv.WriteRecordsAsync(users);
                }
            }

            var csvData = csvBuilder.ToString();
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(csvData));
            var fileName = "UserRegistryExport.csv";

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, base64);
        }

        public async ValueTask DisposeAsync()
        {
            if (dotNetObjectReference != null)
            {
                dotNetObjectReference.Dispose();
            }
            GC.SuppressFinalize(this);
            await Task.CompletedTask;
        }
    }
}
