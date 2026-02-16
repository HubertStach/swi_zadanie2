# Rick and Morty Recruitment API

Projekt API stworzony w ramach zadania rekrutacyjnego. Aplikacja agreguje dane z zewnętrznego [Rick and Morty API](https://rickandmortyapi.com/) i udostępnia rozszerzone możliwości wyszukiwania oraz analizy powiązań między postaciami.


# Technologie
*   **Platforma:** .NET 8 (C# 12)
*   **Framework:** ASP.NET Core Web API
*   **Testy:** xUnit, Moq, FluentAssertions

# Kluczowe Funkcjonalności

### 1. Multi-search (`/search`)
Endpoint umożliwia jednoczesne przeszukiwanie trzech kategorii: postaci, lokacji i odcinków.

### 2. Analiza Powiązań (`/top-pairs`)
Endpoint znajduje pary postaci, które najczęściej pojawiały się wspólnie w odcinkach.
*   **Algorytm:**
    1. Pobranie wszystkich dostępnych odcinków (z obsługą paginacji).
    2. Generowanie unikalnych kombinacji par postaci dla każdego odcinka.
    3. Zliczanie wystąpień przy użyciu `Dictionary<(int, int), int>`.
    4. Pobranie szczegółowych danych tylko dla końcowej listy "Top" postaci (optymalizacja liczby zapytań).
*   **Parametry:** Filtrowanie po liczbie wspólnych odcinków (`min`, `max`) oraz limitowanie wyników.

## Testy Jednostkowe
W projekcie zawarto testy jednostkowe weryfikujące poprawność logiki biznesowej:
*   **Mockowanie:** Zastosowano `Mock<HttpMessageHandler>`, aby izolować testy od zewnętrznego API (testy są szybkie i niezależne od internetu).
*   **Weryfikacja Algorytmu:** Testy sprawdzają, czy serwis poprawnie liczy pary postaci na podstawie spreparowanych danych testowych.
*   **Agregacja:** Testy sprawdzają poprawność łączenia wyników z różnych kategorii w endpointcie `/search`.

Aby uruchomić testy:
```bash
dotnet test
```

## Uruchomienie projektu

### Lokalnie (z zainstalowanym .NET 8 SDK):
1. Przejdź do folderu projektu.
2. Uruchom aplikację:
```bash
dotnet run --project zadanie2/zadanie2.csproj
```
3. Dokumentacja Swagger jest dostępna pod adresem: `http://localhost:5260/swagger` (port może się różnić w zależności od konfiguracji).

### Przez Docker:
1. Zbuduj obraz:
```bash
docker build -t rick-morty-api .
```
2. Uruchom kontener:
```bash
docker run -p 8080:8080 rick-morty-api
```