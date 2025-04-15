# Architektura Projektu

## Struktura Projektu MVVM

Projekt jest zorganizowany zgodnie z wzorcem architektonicznym Model-View-ViewModel (MVVM), co pozwala na separację komponentów i lepszą organizację kodu.

### Model
Klasy w warstwie modelu zawierają podstawową logikę biznesową:
- `Ball` - reprezentuje piłkę poruszającą się w przestrzeni
- `Logger` - odpowiada za logowanie zdarzeń w systemie
- `LoggerManager` - zarządza instancją loggera
- `DataAPI` - API komunikacyjne do obsługi danych

### ViewModel
Klasy w warstwie ViewModel:
- `MainViewModel` - główny model widoku koordynujący interakcje
- `BallViewModel` - model widoku dla piłek
- `LogViewModel` - model widoku dla logów

### View
Komponenty interfejsu użytkownika:
- `MainWindow` - główne okno aplikacji
- `BallsView` - widok prezentujący ruch piłek
- `LogView` - widok prezentujący logi systemu

## Diagram Komunikacji

```
+----------------+      +----------------+      +-----------------+
|                |      |                |      |                 |
|     Model      <----->    ViewModel    <----->      View       |
|                |      |                |      |                 |
+----------------+      +----------------+      +-----------------+
        ^                      ^                       ^
        |                      |                       |
        v                      v                       v
+----------------+      +----------------+      +-----------------+
|                |      |                |      |                 |
|    DataAPI     |      |  BallManager   |      |   UI Controls  |
|                |      |                |      |                 |
+----------------+      +----------------+      +-----------------+
```

## Przepływ Danych

1. Użytkownik wchodzi w interakcję z View
2. View przekazuje akcje do ViewModel
3. ViewModel aktualizuje Model
4. Model wykonuje logikę biznesową
5. Model powiadamia ViewModel o zmianach
6. ViewModel aktualizuje View

## Współbieżność

W projekcie wykorzystywane są następujące mechanizmy współbieżności:
- Wątki do symulacji ruchu piłek
- Blokady do synchronizacji dostępu
- Wzorzec obserwatora do powiadamiania o zmianach stanu
