# Mechanizmy Współbieżności w Projekcie

## Wątki

W projekcie każda piłka jest symulowana w osobnym wątku, co pozwala na równoległe obliczanie ruchu wielu obiektów.

```csharp
// Przykład tworzenia wątku dla piłki
Thread ballThread = new Thread(() => 
{
    while (!cancellationToken.IsCancellationRequested)
    {
        ball.Move();
        Thread.Sleep(timeInterval);
    }
});
ballThread.Start();
```

## Synchronizacja Dostępu

Aby zapobiec problemom związanym z równoczesnym dostępem do wspólnych zasobów, projekt wykorzystuje:

### Blokady (lock)

```csharp
// Przykład użycia blokady
private readonly object _locker = new object();

public void UpdateBallPosition(Ball ball, Vector2 newPosition)
{
    lock (_locker)
    {
        ball.Position = newPosition;
        // Sprawdzanie kolizji, etc.
    }
}
```

### Monitory

```csharp
// Przykład użycia monitora
private readonly object _stateObject = new object();

public void WaitForStateChange()
{
    Monitor.Enter(_stateObject);
    try
    {
        while (!stateChanged)
        {
            Monitor.Wait(_stateObject);
        }
        // Przetwarzanie po zmianie stanu
    }
    finally
    {
        Monitor.Exit(_stateObject);
    }
}

public void SignalStateChange()
{
    lock (_stateObject)
    {
        stateChanged = true;
        Monitor.Pulse(_stateObject);
    }
}
```

### Semafory

```csharp
// Przykład użycia semafora
private SemaphoreSlim _resourceAccess = new SemaphoreSlim(1, 1);

public async Task ProcessResourceAsync()
{
    await _resourceAccess.WaitAsync();
    try
    {
        // Dostęp do zasobu
    }
    finally
    {
        _resourceAccess.Release();
    }
}
```

## Bezpieczna Komunikacja

Do bezpiecznej komunikacji między wątkami wykorzystywane są:

### Kolejki Współbieżne

```csharp
private ConcurrentQueue<LogEntry> _logQueue = new ConcurrentQueue<LogEntry>();

public void EnqueueLog(LogEntry entry)
{
    _logQueue.Enqueue(entry);
}

public bool TryDequeueLog(out LogEntry entry)
{
    return _logQueue.TryDequeue(out entry);
}
```

### Zdarzenia i Delegaty

```csharp
// Przykład użycia zdarzeń do komunikacji między wątkami
public event EventHandler<BallEventArgs> BallMoved;

protected virtual void OnBallMoved(Ball ball)
{
    BallMoved?.Invoke(this, new BallEventArgs(ball));
}
```

## Zakończenie Wątków

Do bezpiecznego zakończenia wątków używany jest token anulowania:

```csharp
private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

public void StartSimulation()
{
    var token = _cancellationTokenSource.Token;
    Task.Run(() => SimulationLoop(token), token);
}

public void StopSimulation()
{
    _cancellationTokenSource.Cancel();
    // Opcjonalnie: Poczekaj na zakończenie wątków
}
```

## Mechanizm Reactive Extensions (Rx.NET)

Dla zaawansowanej obsługi zdarzeń asynchronicznych projekt może wykorzystywać bibliotekę Rx.NET:

```csharp
// Przykład użycia Rx.NET
IObservable<BallPosition> ballPositions = Observable.FromEvent<BallEventArgs>(
    h => BallMoved += h,
    h => BallMoved -= h
).Select(e => new BallPosition(e.Ball.Id, e.Ball.X, e.Ball.Y));

ballPositions.Subscribe(position => 
{
    // Aktualizacja UI
});
```
