Court Booking API

Jednostavan backend za upravljanje sportskim terenima i rezervacijama.

*Pokretanje*

Pokretanje baze:
dotnet ef database update
Pokrentanje aplikacije:
dotnet run
Otvoranje Swaggera:
http://localhost:XXXX/swagger

*Testiranje*

Za testiranje se koristi Swagger.
Aplikacija nema login/autentikaciju, nego se koristi parametar actorUserId kako bi se utvrdilo tko izvršava radnjuu.

Prilikom prvog pokretanja automatski se kreiraju početni podaci:
korisnici:
Id = 1 → Admin
Id = 2, 3 → obični korisnici
tereni:
nekoliko unaprijed unesenih terena (tennis, football, basketball)

*Pravila za rezervacije*

rezervacija mora biti unutar istog dana
moguće je rezervirati samo pune sate (npr. 14:00 – 15:00)
maksimalno trajanje je 2 sata
radno vrijeme je od 07:00 do 23:00
nije moguće napraviti preklapajuće rezervacije

*Format vremena kod unosa u Swagger:*

datum: 2026-05-01
vrijeme: 16:00
Nije potrebno unositi puni ISO format (npr. 16:00:00Z).

Primjer rezervacije:
POST /bookings

{
"courtId": 1,
"actorUserId": 2,
"startTime": "2026-05-01T10:00",
"endTime": "2026-05-01T11:00"
}
