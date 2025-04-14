# Szczegółowy Opis Przepływu Programu

## Inicjalizacja Aplikacji

1. **Rozpoczęcie Programu**
   - Utworzenie instancji aplikacji WPF
   - Inicjalizacja głównego okna `MainWindow`
   - Konfiguracja kontenera Dependency Injection (jeśli jest używany)

2. **Inicjalizacja Modelu**
   - Utworzenie instancji `LoggerManager`
   - Inicjalizacja `DataAPI`
   - Utworzenie początkowych obiektów `Ball`

3. **Inicjalizacja ViewModel**
   - Utworzenie `MainViewModel` z referencjami do modelu
   - Inicjalizacja wiązań do właściwości modelu
   - Konfiguracja komend

4. **Inicjalizacja View**
   - Powiązanie View z ViewModel poprzez DataContext
   - Renderowanie początkowego interfejsu użytkownika

## Normalne Działanie

1. **Aktualizacja Stanu Piłek**
   - Każda piłka jest symulowana w osobnym wątku
   - W regularnych interwałach obliczana jest nowa pozycja
   - ViewModel jest powiadamiany o zmianach przez zdarzenia PropertyChanged
   - View automatycznie aktualizuje pozycje piłek dzięki Data Binding

2. **Obsługa Kolizji**
   - Wykrywanie kolizji między piłkami
   - Obliczenie nowych wektorów prędkości po kolizji
   - Aktualizacja stanu piłek
   - Logowanie zdarzeń kolizji

3. **Logowanie**
   - Każde znaczące zdarzenie jest rejestrowane przez `Logger`
   - `LogViewModel` obserwuje zmiany w logu
   - `LogView` wyświetla aktualizowane wpisy logów

## Zakończenie Działania

1. **Zatrzymanie Symulacji**
   - Sygnalizacja do wątków o konieczności zakończenia
   - Oczekiwanie na bezpieczne zatrzymanie wątków
   - Zwolnienie zasobów

2. **Zapisanie Stanu**
   - Opcjonalny zapis stanu do późniejszego przywrócenia

3. **Zamknięcie Aplikacji**
   - Zwolnienie wszystkich zasobów
   - Zamknięcie interfejsu użytkownika
   - Zakończenie procesu aplikacji
