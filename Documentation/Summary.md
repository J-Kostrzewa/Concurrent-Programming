# Podsumowanie Projektu Programowania Współbieżnego

## Cele projektu

Projekt ma na celu demonstrację mechanizmów programowania współbieżnego poprzez symulację ruchu piłek w przestrzeni dwuwymiarowej. Każda piłka działa w osobnym wątku, co pozwala na równoległe obliczanie ich ruchów i obsługę kolizji. Projekt wykorzystuje wzorzec architektoniczny MVVM do strukturyzacji kodu.

## Główne funkcje

1. **Symulacja wielu piłek** - równoczesny ruch wielu obiektów symulowany w osobnych wątkach
2. **Wykrywanie kolizji** - obsługa interakcji między piłkami
3. **System logowania** - rejestracja zdarzeń podczas działania aplikacji
4. **Responsywny interfejs użytkownika** - niezależny od logiki symulacji

## Wyzwania i rozwiązania

### Synchronizacja dostępu
Problem współdzielonego dostępu do piłek i ich stanu rozwiązano za pomocą blokad (`lock`), zapobiegających równoczesnemu dostępowi do kolekcji piłek podczas ich aktualizacji i sprawdzania kolizji.

### Aktualizacja UI
Dzięki zastosowaniu wzorca MVVM i implementacji `INotifyPropertyChanged`, aktualizacje stanu piłek są automatycznie propagowane do interfejsu użytkownika bez blokowania wątku UI.

### Zakańczanie wątków
Do bezpiecznego zatrzymywania wątków symulacji zastosowano tokeny anulowania (`CancellationToken`), co pozwala na eleganckie zamykanie aplikacji.

## Zastosowane technologie i wzorce

1. **WPF** - framework do tworzenia interfejsu użytkownika
2. **MVVM** - wzorzec architektoniczny separujący dane od prezentacji
3. **Task Parallel Library** - do zarządzania operacjami asynchronicznymi
4. **INotifyPropertyChanged** - mechanizm powiadamiania o zmianach właściwości
5. **Singleton** - dla klasy Logger, zapewniający jeden punkt logowania

## Możliwe rozszerzenia

1. **Dodanie parametryzacji symulacji** - możliwość zmiany liczby piłek, ich prędkości, itp.
2. **Wizualizacja danych symulacji** - wykresy, statystyki zderzeń
3. **Zaawansowana fizyka** - dodanie grawitacji, tarcia, itp.
4. **Zapisywanie i odtwarzanie symulacji** - możliwość zachowania stanu
5. **Obsługa gestów użytkownika** - interakcja z symulacją w czasie rzeczywistym

## Wnioski

Projekt demonstruje kluczowe aspekty programowania współbieżnego, w tym zarządzanie wątkami, synchronizację dostępu do zasobów oraz komunikację między komponentami współbieżnymi. Zastosowanie wzorca MVVM znacząco ułatwia organizację kodu i separację odpowiedzialności, co jest szczególnie istotne w aplikacjach współbieżnych.
