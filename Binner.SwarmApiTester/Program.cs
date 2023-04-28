// See https://aka.ms/new-console-template for more information

using Binner.SwarmApi;

Console.WriteLine("Swarm Api Test Console");
Console.WriteLine("======================");

var config = new SwarmApiConfiguration();
var client = new SwarmApiClient(config);

for (var i = 0; i < 20; i++)
{
    var response = await client.GetPartInformationAsync("LM358");

    Console.WriteLine("Response:");
    Console.WriteLine("---------------");

    Console.WriteLine($"IsSuccessful: {response.IsSuccessful}");
    Console.WriteLine($"IsRequestThrottled: {response.IsRequestThrottled}");
    if (response.IsRequestThrottled)
    {
        Console.WriteLine($"RetryIn: {response.RetryIn}");
        await Task.Delay(response.RetryIn);
        Console.WriteLine("-------------------");
        Console.WriteLine($"Retrying... (Retry counter = {i + 1})");
        continue;
    }

    if (response.IsSuccessful)
    {
        if (response.Response != null)
        {
            Console.WriteLine($"\nData\n-------------------");
            foreach (var part in response.Response.Parts.OrderByDescending(x => x.QuantityAvailable).Take(5))
            {
                Console.WriteLine($"{part.BasePartNumber}: {part.PartType}");
                Console.WriteLine($"  Cost: {part.Cost}");
                Console.WriteLine($"  Supplier: {part.Supplier}");
                Console.WriteLine($"  Description: {part.Description}");
                Console.WriteLine($"  Mfr: {part.Manufacturer} = {part.ManufacturerPartNumber}");
                Console.WriteLine($"  Keywords: {string.Join(", ", part.Keywords)}");
                Console.WriteLine($"  Datasheets: {string.Join(", ", part.DatasheetUrls)}");
                Console.WriteLine($"  QuantityAvailable: {part.QuantityAvailable}");
                Console.WriteLine();
            }
        }
    }
    else
    {
        var x = 0;
        foreach (var error in response.Errors)
        {
            x++;
            Console.WriteLine($" [{x}] {error}");
        }
    }
    break;
}