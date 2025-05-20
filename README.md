# Walory

## How to run

Install docker [Docker Desktop: The #1 Containerization Tool for Developers | Docker](https://www.docker.com/products/docker-desktop/)

Download this repo: 

> git clone github.com/KasmyrA/Walory

Get into right folder inside the terminal:

> cd Walory\backend\Walory Backend

Run backend via docker compose:

> docker compose up

Or 

> docker compose up --build

Now you may visit [Swagger UI](http://localhost:8080/swagger/index.html) at 127.0.0.0:8080/swagger/index.html

# Portal dla kolekcjonerów

Celem projektu jest stworzenie systemu służącego do ewidencjonowania walorów (inaczej przedmiotów
kolekcjonerskich) przez kolekcjonerów. System ma pozwolić na wprowadzanie przez kolekcjonera nowych
walorów do kolekcji, ich edycji, czy też usuwania. Użytkownik ma możliwość podziału kolekcji na kategorie (np. monety, znaczki, etc.). Tworząc nową kategorię walorów można stworzyć model danych
opisujących dany walor np. rocznik, stan zachowania, zakład wytwórczy itp. Dodatkowo należy zapewnić filtrację i sortowanie walorów wg wybranych cech w celu ich przeglądania. Kolekcjonerzy będą
mieli możliwość zapraszania się (po adresie email) do grona znajomych w celu przeglądania swoich
kolekcji. Wówczas możliwe będzie przeglądanie kolejcji osoby zaproszonej i dodawanie komentarzy pod
konretnymi walorami. Jako dodatkową funkcjonalność można zaimplementować komunikator pomiędzy
znajomymi.

Funkcjonalności

1. System posiada mechanizm tworzenia konta oraz logowania.
2. Użytkownik ma możliwość dodawania, edycji oraz usuwania walorów.
3. Użytkownik ma możliwość wyboru istnitejących kategorii albo dodania nowej jeśli taka nie istnieje.
4. Użytkownik dodając nową kategorię ma możliwość określenia modelu danych ją opisującących.
5. Walory są tworzone w ramach kategorii (np. monety, znaczki, biżuteria, etc.).
6. Użytkownik ma możliwość wysłać zaproszenie (po adresie email) do grona znajomych innemu użytkownikowi systemu. Po akceptacji, użytkownicy mają możliwość przeglądania swoich kolekcji.
7. Użytkownik ma możliwość filtracji oraz sortowania walorów.
8. Użytkownik ma możliwość eksportu danych o walorach (np. po filtracji lub sortowaniu).

Założenia niefunkcjonalne

1. Eksport danych o walorach w formacjie PDF lub xlsx.
2. Dane gromadzone w realcyjnej bazie danych.

# 