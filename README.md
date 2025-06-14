Platforma de Delivery

Vicaș Antonio-Samir
Tișcă Laurențiu Ștefan

Structura în subproiecte
  QuickDelivery.Api
  QuickDelivery.Core
  QuickDelivery.Database
  QuickDelivery.Infrastructure

Configurare EF Core și entități
  Entități definite: User, Order, OrderItem, Product, Partner, Delivery, Payment, Address
  Relațiile configurate în ApplicationDbContext.cs
  Connection string și migrații configurate

Controller, serviciu, repository cu DI
  DI configurat în Program.cs
  Interfețe create pentru servicii și repositories
  TO DO: Implementări goale - toate serviciile și repositories sunt clase goale
  AuthController implementat - dar celelalte sunt goale

Swagger/Testare
  Swagger configurat în Program.cs
  AuthController poate fi testat (register/login/me)
  TO DO: Endpoint-ul principal nu există încă

Endpoint many-to-many ❌
  Nu există endpoint pentru extragerea datelor cu relația many-to-many
  Controllers-ele principale sunt goale


Mapare în servicii ❌
  Serviciile sunt clase goale
  Nu există logică de procesare/mapare
