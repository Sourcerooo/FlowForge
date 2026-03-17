# TODO-Liste -- FlowForge

Arbeitsdokument fuer konkrete Aufgaben, naechste Schritte und Architektur-Follow-ups fuer den
Aufbau von FlowForge als Digital-Twin- und Simulationsanwendung.

Dieses Dokument ist bewusst operativer als `docs/Roadmap.md`.

**Legende Status:** `Offen` · `In Arbeit` · `Blockiert` · `Erledigt`

## Inhaltsverzeichnis

- [Naechste Schritte](#naechste-schritte)
- [Hohe Prioritaet](#hohe-prioritaet)
- [Mittlere Prioritaet](#mittlere-prioritaet)
- [Niedrige Prioritaet](#niedrige-prioritaet)
- [Architekturentscheidungen](#architekturentscheidungen)
- [Technische Schulden](#technische-schulden)
- [Erledigt](#erledigt)

---

## Naechste Schritte

| ID | Status | Aufgabe | Beschreibung |
|---|---|---|---|
| #001 | Offen | MVP-Domaenenmodell konkretisieren | Begriffe wie `Order`, `Station`, `ScenarioDefinition`, `WorkerCapacity` und ihre Regeln festlegen |
| #002 | Offen | Zielstruktur der Solution entscheiden | Festlegen, wann `FlowForge.Simulation`, `FlowForge.Contracts` und `FlowForge.Desktop` als Projekte angelegt werden |
| #003 | Offen | Diskrete Ereignissimulation spezifizieren | Eventtypen, Runner-Verhalten, Queue-Regeln und Simulationszeit definieren |
| #003A | In Arbeit | Event- und Statusmodell reviewen | Vorschlag fuer stationsspezifische Events und Statusuebergaenge im Architekturdokument reviewen und verfeinern |
| #003B | Offen | Event-Payloads definieren | Gemeinsame Basisfelder und spezifische Payloads fuer Generate-, Queue-, Start-, Complete-Events festlegen |
| #004 | Offen | Snapshot-Vertrag definieren | Inhalt, Granularitaet und Aktualisierungsstrategie der immutable snapshots festlegen |
| #005 | Offen | KPI-Schnitt definieren | Kennzahlen fuer Throughput, Lead Time, WIP, Queue Length und Utilization exakt beschreiben |

---

## Hohe Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #010 | Offen | MVP-Prozess fachlich festzurren | Linearer Flow `Source -> Picking -> Packing -> Shipping` mit klaren Statuswechseln definieren | #001 |
| #010A | In Arbeit | Order-Statusuebergaenge definieren | `Created` als externer Einstieg sowie interne Queue-/Processing-Status fachlich abstimmen | #010 |
| #011 | Offen | Simulationszustand modellieren | Mutable Runtime-Zustaende fuer Orders, Stationen und globale Zaehler abgrenzen | #003 |
| #011A | Offen | GenerateSimulationEvent spezifizieren | Batch-Fenster, Erzeugungslogik, Abbruchbedingung und Folgeplanung des Generators definieren | #003 |
| #012 | Offen | Event-Handler-Schnitt spezifizieren | Verantwortlichkeiten von Dispatcher, Scheduler und Handlern dokumentieren | #003 |
| #013 | Offen | Snapshot-DTOs entwerfen | Station-, Order-, KPI- und Alert-Sichten fuer Desktop und API vereinheitlichen | #004 |
| #014 | Erledigt | Delivery-Strategie festgelegt | Desktop-first ist gesetzt, eine API wird frueh aufgebaut und soll moeglichst dieselben Kernschnittstellen nutzen | -- |
| #015 | Offen | Persistenzstrategie fuer Szenarien waehlen | JSON-Dateien fuer MVP bestaetigen oder frueh eine DB-Option vorbereiten | -- |
| #016 | Offen | Teststrategie fuer Simulation festlegen | Unit- und spaetere Integrationstests fuer Runner, Events und KPI-Berechnung planen | #003 |

---

## Mittlere Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #020 | Offen | Application-Use-Cases zuschneiden | `Start`, `Pause`, `Reset`, `LoadScenario`, `GetLatestSnapshot` und KPI-Queries konkret beschreiben | #004 |
| #021 | Offen | CLI-Rolle schaerfen | Festlegen, ob CLI nur Debug-/Dev-Werkzeug oder auch Demo-/Admin-Werkzeug wird | #020 |
| #022 | Offen | Stoerungsmodell planen | Ausfaelle, Shipping-Stopp oder Lastspitzen fuer das erste Erweiterungsinkrement priorisieren | #010 |
| #023 | Offen | Replay-/Timeline-Bedarf klaeren | Entscheiden, ob gespeicherte Simulationslaeufe fuer die erste Produktstufe wichtig sind | #015 |
| #024 | Offen | UI-Datenaktualisierung definieren | Snapshot-Polling oder spaeteres Streaming-Konzept fuer Desktop/API dokumentieren | #013 |
| #025 | Offen | Demo-Szenarien sammeln | Mindestens ein Basisszenario und ein Engpass-Szenario fachlich beschreiben | #010 |

---

## Niedrige Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #030 | Offen | Erweiterbare Flussmodelle vorbereiten | Branching, Rework oder Prioritaetsorders als spaetere Zielbilder dokumentieren | #010 |
| #031 | Offen | Export-Faehigkeiten planen | KPI-Export, Run-Export oder Reporting fuer spaetere Milestones vorbereiten | #023 |
| #032 | Offen | Fruehe API-Schnitt vorbereiten | REST-Vertrag frueh mit denselben Kern-Datenmodellen wie fuer den Desktop vorbereiten | #024 |
| #033 | Offen | Mockups mit Vertragsmodell abgleichen | Sicherstellen, dass UI-Mockups mit Snapshot- und KPI-Modell zusammenpassen | #013 |

---

## Architekturentscheidungen

| ID | Status | Entscheidung | Beschreibung |
|---|---|---|---|
| #040 | Erledigt | Erster Referenzclient | Desktop-first mit frueher API-Unterstuetzung und gemeinsamen Kernschnittstellen |
| #041 | Offen | Persistenz fuer MVP | Reine Dateibasis oder bereits vorbereitete relationale Speicherung |
| #042 | Offen | Stoerungen im MVP | Normale Prozesssimulation zuerst oder frueh mit Ausfaellen/Spitzenlast erweitern |
| #043 | Offen | Snapshot-Takt | Fester Publikationsintervall oder ereignis-/zustandsgetriebene Publikation |

---

## Technische Schulden

| ID | Status | Thema | Beschreibung |
|---|---|---|---|
| #050 | Offen | Template-Denken im Code abbauen | Vorhandene Platzhalter in API, CLI, Application und Tests spaeter systematisch auf die Produktidee ausrichten |
| #051 | Offen | Dokumentation synchron halten | Architektur, README, Roadmap und Todo bei jeder groesseren Richtungsentscheidung aktualisieren |
| #052 | Offen | Zielstruktur sauber migrieren | Beim Hinzufuegen neuer Projekte bestehende Abhaengigkeitsrichtung sauber halten |

---

## Erledigt

| ID | Status | Aufgabe | Beschreibung |
|---|---|---|---|
| #000 | Erledigt | Dokumentationsbasis neu ausgerichtet | README, Architektur, Roadmap und Todo auf die FlowForge-Digital-Twin-Vision umgestellt |
| #001A | Erledigt | Delivery-Priorisierung entschieden | Desktop-first mit frueher API und gemeinsamem Vertragsmodell festgelegt |
