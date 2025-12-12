using System.Timers;
using Timer = System.Timers.Timer;

namespace MosaicPortal.Web.Services;

/// <summary>
/// Service for managing dashboard data with real-time updates and date filtering
/// </summary>
public class DashboardService : IDisposable
{
    private readonly Timer _updateTimer;
    private readonly Random _random = new();

    public event Action? OnDataUpdated;
    public event Action<DashboardData>? OnDashboardDataChanged;

    public DashboardData CurrentData { get; private set; } = new();
    public DateTime? FilterStartDate { get; private set; }
    public DateTime? FilterEndDate { get; private set; }
    public bool IsRealTimeEnabled { get; private set; } = false;
    public int UpdateIntervalMs { get; private set; } = 5000;

    public DashboardService()
    {
        _updateTimer = new Timer(UpdateIntervalMs);
        _updateTimer.Elapsed += OnTimerElapsed;
        InitializeData();
    }

    private void InitializeData()
    {
        CurrentData = new DashboardData
        {
            // Sales Cards
            AcmePlusSales = 24780,
            AcmePlusChange = 49,
            AcmeAdvancedSales = 17489,
            AcmeAdvancedChange = -14,
            AcmeProfessionalSales = 9962,
            AcmeProfessionalChange = 29,

            // Direct vs Indirect
            DirectSales = 5467,
            IndirectSales = 3489,

            // Real-time value
            RealTimeValue = 35.65m,

            // Top Countries
            TopCountries = new List<CountryData>
            {
                new() { Country = "United States", Value = 7500, Percentage = 35 },
                new() { Country = "Italy", Value = 4000, Percentage = 19 },
                new() { Country = "Germany", Value = 3500, Percentage = 16 },
                new() { Country = "United Kingdom", Value = 2500, Percentage = 12 },
                new() { Country = "Other", Value = 4000, Percentage = 18 }
            },

            // Top Channels
            TopChannels = new List<ChannelData>
            {
                new() { Source = "Google", Visitors = 2890, Revenues = 14249, SalesPercent = 94, ConversionRate = 3.2m },
                new() { Source = "Twitter", Visitors = 2767, Revenues = 13524, SalesPercent = 87, ConversionRate = 2.8m },
                new() { Source = "GitHub", Visitors = 2374, Revenues = 11234, SalesPercent = 79, ConversionRate = 2.5m },
                new() { Source = "Vimeo", Visitors = 2091, Revenues = 9890, SalesPercent = 72, ConversionRate = 2.1m },
                new() { Source = "Pinterest", Visitors = 1480, Revenues = 7521, SalesPercent = 68, ConversionRate = 1.9m }
            },

            // Sales over time (monthly)
            MonthlySales = new List<MonthlySalesData>
            {
                new() { Month = "Jan", Current = 8000, Previous = 6800 },
                new() { Month = "Feb", Current = 12500, Previous = 7200 },
                new() { Month = "Mar", Current = 7800, Previous = 8100 },
                new() { Month = "Apr", Current = 14200, Previous = 9400 },
                new() { Month = "May", Current = 11800, Previous = 11800 },
                new() { Month = "Jun", Current = 16400, Previous = 12200 },
                new() { Month = "Jul", Current = 18500, Previous = 14200 },
                new() { Month = "Aug", Current = 21000, Previous = 15800 },
                new() { Month = "Sep", Current = 19400, Previous = 17100 },
                new() { Month = "Oct", Current = 22500, Previous = 19200 },
                new() { Month = "Nov", Current = 24200, Previous = 21400 },
                new() { Month = "Dec", Current = 26800, Previous = 23200 }
            },

            // Recent activity
            RecentActivities = new List<ActivityData>
            {
                new() { User = "Nick Mark", Action = "purchased Mosaic-Design", Amount = 249, Time = DateTime.Now.AddMinutes(-2) },
                new() { User = "Sara Mitha", Action = "subscribed to Starter Plan", Amount = 49, Time = DateTime.Now.AddMinutes(-15) },
                new() { User = "John Smith", Action = "purchased Starter Pack", Amount = 79, Time = DateTime.Now.AddHours(-1) },
                new() { User = "Lisa Wong", Action = "upgraded to Pro Plan", Amount = 129, Time = DateTime.Now.AddHours(-2) },
                new() { User = "Mike Davis", Action = "renewed subscription", Amount = 99, Time = DateTime.Now.AddHours(-5) }
            },

            // Income/Expenses
            TotalIncome = 57849,
            TotalExpenses = 12389,
            IncomeChange = 8.2m,
            ExpenseChange = -2.1m,

            LastUpdated = DateTime.Now
        };
    }

    public void SetDateFilter(DateTime? startDate, DateTime? endDate)
    {
        FilterStartDate = startDate;
        FilterEndDate = endDate;
        RefreshData();
    }

    public void ClearDateFilter()
    {
        FilterStartDate = null;
        FilterEndDate = null;
        RefreshData();
    }

    public void EnableRealTimeUpdates(int intervalMs = 5000)
    {
        UpdateIntervalMs = intervalMs;
        _updateTimer.Interval = intervalMs;
        IsRealTimeEnabled = true;
        _updateTimer.Start();
    }

    public void DisableRealTimeUpdates()
    {
        IsRealTimeEnabled = false;
        _updateTimer.Stop();
    }

    public void RefreshData()
    {
        // Simulate data refresh based on date filter
        if (FilterStartDate.HasValue && FilterEndDate.HasValue)
        {
            var daysDiff = (FilterEndDate.Value - FilterStartDate.Value).Days;
            var multiplier = Math.Max(0.5, Math.Min(2.0, daysDiff / 30.0));

            CurrentData.AcmePlusSales = (int)(24780 * multiplier);
            CurrentData.AcmeAdvancedSales = (int)(17489 * multiplier);
            CurrentData.AcmeProfessionalSales = (int)(9962 * multiplier);
            CurrentData.DirectSales = (int)(5467 * multiplier);
            CurrentData.IndirectSales = (int)(3489 * multiplier);
            CurrentData.TotalIncome = (int)(57849 * multiplier);
            CurrentData.TotalExpenses = (int)(12389 * multiplier);
        }

        CurrentData.LastUpdated = DateTime.Now;
        OnDataUpdated?.Invoke();
        OnDashboardDataChanged?.Invoke(CurrentData);
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        // Simulate real-time updates with small random variations
        CurrentData.RealTimeValue += (decimal)(_random.NextDouble() * 2 - 1);
        CurrentData.RealTimeValue = Math.Max(0, CurrentData.RealTimeValue);

        // Occasionally update other metrics
        if (_random.Next(100) < 20)
        {
            CurrentData.AcmePlusSales += _random.Next(-50, 150);
            CurrentData.DirectSales += _random.Next(-10, 30);
            CurrentData.IndirectSales += _random.Next(-5, 20);
        }

        // Add new activity occasionally
        if (_random.Next(100) < 10)
        {
            var names = new[] { "John Doe", "Jane Smith", "Bob Wilson", "Alice Brown", "Charlie Lee" };
            var actions = new[] { "purchased Starter Pack", "subscribed to Pro Plan", "renewed subscription", "upgraded plan" };
            var amounts = new[] { 49, 79, 99, 129, 199, 249 };

            var newActivity = new ActivityData
            {
                User = names[_random.Next(names.Length)],
                Action = actions[_random.Next(actions.Length)],
                Amount = amounts[_random.Next(amounts.Length)],
                Time = DateTime.Now
            };

            CurrentData.RecentActivities.Insert(0, newActivity);
            if (CurrentData.RecentActivities.Count > 10)
                CurrentData.RecentActivities.RemoveAt(CurrentData.RecentActivities.Count - 1);
        }

        CurrentData.LastUpdated = DateTime.Now;
        OnDataUpdated?.Invoke();
        OnDashboardDataChanged?.Invoke(CurrentData);
    }

    public byte[] ExportToCsv()
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Metric,Value,Change %");
        csv.AppendLine($"Acme Plus Sales,{CurrentData.AcmePlusSales},{CurrentData.AcmePlusChange}");
        csv.AppendLine($"Acme Advanced Sales,{CurrentData.AcmeAdvancedSales},{CurrentData.AcmeAdvancedChange}");
        csv.AppendLine($"Acme Professional Sales,{CurrentData.AcmeProfessionalSales},{CurrentData.AcmeProfessionalChange}");
        csv.AppendLine($"Direct Sales,{CurrentData.DirectSales},");
        csv.AppendLine($"Indirect Sales,{CurrentData.IndirectSales},");
        csv.AppendLine($"Real Time Value,{CurrentData.RealTimeValue},");
        csv.AppendLine($"Total Income,{CurrentData.TotalIncome},{CurrentData.IncomeChange}");
        csv.AppendLine($"Total Expenses,{CurrentData.TotalExpenses},{CurrentData.ExpenseChange}");

        csv.AppendLine();
        csv.AppendLine("Top Countries");
        csv.AppendLine("Country,Value,Percentage");
        foreach (var country in CurrentData.TopCountries)
        {
            csv.AppendLine($"{country.Country},{country.Value},{country.Percentage}");
        }

        csv.AppendLine();
        csv.AppendLine("Top Channels");
        csv.AppendLine("Source,Visitors,Revenues,Sales %,Conversion Rate");
        foreach (var channel in CurrentData.TopChannels)
        {
            csv.AppendLine($"{channel.Source},{channel.Visitors},{channel.Revenues},{channel.SalesPercent},{channel.ConversionRate}");
        }

        return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
    }

    public byte[] ExportToJson()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(CurrentData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public void Dispose()
    {
        _updateTimer.Stop();
        _updateTimer.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class DashboardData
{
    // Sales Cards
    public int AcmePlusSales { get; set; }
    public int AcmePlusChange { get; set; }
    public int AcmeAdvancedSales { get; set; }
    public int AcmeAdvancedChange { get; set; }
    public int AcmeProfessionalSales { get; set; }
    public int AcmeProfessionalChange { get; set; }

    // Direct vs Indirect
    public int DirectSales { get; set; }
    public int IndirectSales { get; set; }

    // Real-time
    public decimal RealTimeValue { get; set; }

    // Top Countries
    public List<CountryData> TopCountries { get; set; } = new();

    // Top Channels
    public List<ChannelData> TopChannels { get; set; } = new();

    // Monthly Sales
    public List<MonthlySalesData> MonthlySales { get; set; } = new();

    // Recent Activity
    public List<ActivityData> RecentActivities { get; set; } = new();

    // Income/Expenses
    public int TotalIncome { get; set; }
    public int TotalExpenses { get; set; }
    public decimal IncomeChange { get; set; }
    public decimal ExpenseChange { get; set; }

    public DateTime LastUpdated { get; set; }
}

public class CountryData
{
    public string Country { get; set; } = "";
    public int Value { get; set; }
    public int Percentage { get; set; }
}

public class ChannelData
{
    public string Source { get; set; } = "";
    public int Visitors { get; set; }
    public int Revenues { get; set; }
    public int SalesPercent { get; set; }
    public decimal ConversionRate { get; set; }
}

public class MonthlySalesData
{
    public string Month { get; set; } = "";
    public int Current { get; set; }
    public int Previous { get; set; }
}

public class ActivityData
{
    public string User { get; set; } = "";
    public string Action { get; set; } = "";
    public int Amount { get; set; }
    public DateTime Time { get; set; }
}
