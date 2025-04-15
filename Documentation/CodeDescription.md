# Szczegółowy Opis Kodu

## Model

### Ball.cs

Ball to klasa reprezentująca piłkę poruszającą się w przestrzeni dwuwymiarowej:

```csharp
// Konstruktor inicjalizuje piłkę z początkową pozycją, prędkością i rozmiarem
public Ball(double x, double y, double velocityX, double velocityY, double radius)
{
    X = x;
    Y = y;
    VelocityX = velocityX;
    VelocityY = velocityY;
    Radius = radius;
}

// Metoda Move aktualizuje pozycję piłki na podstawie jej prędkości
// Zawiera również logikę odbijania od granic obszaru
public void Move(double areaWidth, double areaHeight)
{
    // Obliczenie nowej pozycji
    X += VelocityX;
    Y += VelocityY;

    // Sprawdzenie kolizji z granicami obszaru
    if (X - Radius < 0 || X + Radius > areaWidth)
    {
        VelocityX = -VelocityX;
        X = Math.Clamp(X, Radius, areaWidth - Radius);
    }

    if (Y - Radius < 0 || Y + Radius > areaHeight)
    {
        VelocityY = -VelocityY;
        Y = Math.Clamp(Y, Radius, areaHeight - Radius);
    }

    // Powiadomienie o zmianie właściwości
    OnPropertyChanged(nameof(X));
    OnPropertyChanged(nameof(Y));
}

// Implementacja INotifyPropertyChanged dla aktualizacji UI
public event PropertyChangedEventHandler PropertyChanged;

protected virtual void OnPropertyChanged(string propertyName)
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

### Logger.cs

Logger to klasa odpowiedzialna za zapisywanie komunikatów diagnostycznych:

```csharp
// Singleton zapewniający jeden punkt logowania w aplikacji
private static Logger _instance;
private static readonly object _lock = new object();

public static Logger Instance
{
    get
    {
        lock (_lock)
        {
            return _instance ??= new Logger();
        }
    }
}

// Metoda logująca zdarzenie z określeniem poziomu ważności
public void Log(LogLevel level, string message)
{
    var logEntry = new LogEntry
    {
        Timestamp = DateTime.Now,
        Level = level,
        Message = message
    };

    // Dodanie wpisu do kolekcji
    lock (_lock)
    {
        LogEntries.Add(logEntry);
        LogAdded?.Invoke(this, logEntry);
    }
}

// Zdarzenie informujące o dodaniu nowego logu
public event EventHandler<LogEntry> LogAdded;
```

## ViewModel

### MainViewModel.cs

MainViewModel to główny model widoku, który koordynuje działanie całej aplikacji:

```csharp
public class MainViewModel : INotifyPropertyChanged
{
    private readonly Timer _simulationTimer;
    private readonly List<Ball> _balls = new List<Ball>();
    private readonly object _ballsLock = new object();

    // Konstruktor inicjalizuje symulację
    public MainViewModel()
    {
        // Utworzenie początkowych piłek
        CreateInitialBalls();

        // Konfiguracja timera symulacji
        _simulationTimer = new Timer(SimulationTick, null, 0, 16); // ~60 FPS
    }

    // Metoda wywoływana przez timer - główna pętla symulacji
    private void SimulationTick(object state)
    {
        lock (_ballsLock)
        {
            // Aktualizacja pozycji wszystkich piłek
            foreach (var ball in _balls)
            {
                ball.Move(CanvasWidth, CanvasHeight);
            }

            // Sprawdzenie kolizji między piłkami
            CheckCollisions();
        }
    }

    // Sprawdzanie kolizji między wszystkimi parami piłek
    private void CheckCollisions()
    {
        for (int i = 0; i < _balls.Count; i++)
        {
            for (int j = i + 1; j < _balls.Count; j++)
            {
                if (BallsCollide(_balls[i], _balls[j]))
                {
                    ResolveBallCollision(_balls[i], _balls[j]);
                    Logger.Instance.Log(LogLevel.Info, $"Collision between Ball {i} and Ball {j}");
                }
            }
        }
    }

    // Implementacja INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## View

### MainWindow.xaml

```xml
<Window x:Class="ConcurrentBallsSimulation.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Concurrent Balls Simulation" Height="600" Width="800">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="150"/>
        </Grid.RowDefinitions>
        
        <!-- Canvas do wyświetlania piłek -->
        <Canvas Grid.Row="0" Background="LightBlue">
            <!-- Piłki są renderowane tutaj przez ItemsControl -->
            <ItemsControl ItemsSource="{Binding Balls}">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <Canvas/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemContainerStyle>
                    <Style TargetType="ContentPresenter">
                        <Setter Property="Canvas.Left" Value="{Binding X, Converter={StaticResource BallPositionConverter}}"/>
                        <Setter Property="Canvas.Top" Value="{Binding Y, Converter={StaticResource BallPositionConverter}}"/>
                    </Style>
                </ItemsControl.ItemContainerStyle>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Ellipse Width="{Binding Diameter}" Height="{Binding Diameter}"
                                 Fill="{Binding Color}"/>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </Canvas>
        
        <!-- Panel logów -->
        <Grid Grid.Row="1">
            <ListView ItemsSource="{Binding LogEntries}">
                <ListView.View>
                    <GridView>
                        <GridViewColumn Header="Time" DisplayMemberBinding="{Binding Timestamp, StringFormat=HH:mm:ss.fff}"/>
                        <GridViewColumn Header="Level" DisplayMemberBinding="{Binding Level}"/>
                        <GridViewColumn Header="Message" DisplayMemberBinding="{Binding Message}"/>
                    </GridView>
                </ListView.View>
            </ListView>
        </Grid>
    </Grid>
</Window>
```
