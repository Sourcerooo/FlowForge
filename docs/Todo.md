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
| #002 | In Arbeit | Zielstruktur der Solution entscheiden | `FlowForge.Simulation` ist angelegt; Checkpoint-Vertraege liegen wieder in `Simulation` und `Application`; naechster struktureller Schritt ist die Einplanung von `FlowForge.Desktop` |
| #003 | Offen | Diskrete Ereignissimulation spezifizieren | Eventtypen, Runner-Verhalten, Queue-Regeln und Simulationszeit definieren |
| #003A | In Arbeit | Event- und Statusmodell reviewen | Vorschlag fuer generische Queue-/Start-/Complete-Events und Statusuebergaenge im Architekturdokument reviewen und verfeinern |
| #003B | In Arbeit | Event-Payloads definieren | Gemeinsame Basisfelder wie `SequenceNumber`, `SortRank`, `SimulationRunId`, `ProcessingToken` und spezifische Payloads festlegen |
| #003C | In Arbeit | Event-Routing definieren | Routing ueber `EventKind`, `ProcessStage` und optionale Subtypen konkret festlegen |
| #003D | In Arbeit | Event-Basiskontrakt verfeinern | Basisklasse bzw. Record-Form, Pflichtfelder und Enum-Modell fuer Simulationsevents abstimmen |
| #003E | Offen | Event-Vokabular auf WorkItem pruefen | Entscheiden, ob Queue-/Complete-Events bereits im MVP von `Order...` auf `WorkItem...` umbenannt werden |
| #004 | In Arbeit | Snapshot-Vertrag definieren | Inhalt, Granularitaet, Ownership und Aktualisierungsstrategie der immutable snapshots festlegen |
| #004A | In Arbeit | Snapshot-Datenmodell verfeinern | Root-Snapshot, Scenario-, Process-, Station-, Order-, KPI-, Alert- und Metadata-Strukturen konkretisieren |
| #004B | In Arbeit | Copy-vs-Reference-Regeln festlegen | Definieren, welche Daten in Snapshots kopiert und welche run-scoped immutable Daten referenziert werden duerfen |
| #004C | In Arbeit | Snapshot-Store entwerfen | Owner, latest-snapshot-Swap, Timeline-Speicherung und Leserzugriff fuer Desktop/API festlegen |
| #004D | In Arbeit | Desktop-Playback-Timeline festlegen | Definieren, wie die UI Snapshots langsamer als die Simulation abspielt |
| #004E | In Arbeit | Checkpoint-Vertrag definieren | `SimulationStateDocument` liegt in `FlowForge.Simulation`, das Speicher-Interface in `FlowForge.Application`; Builder- und weitere Resume-Implementierungen folgen |
| #005 | In Arbeit | KPI-Schnitt definieren | Kennzahlen fuer Throughput, Lead Time, WIP, Queue Length, Utilization und Bottleneck exakt beschreiben |
| #005A | In Arbeit | KPI-Formeln festlegen | Rohwerte, Aggregationen und Formeln fuer alle MVP-KPIs verbindlich dokumentieren |
| #005B | In Arbeit | KPI-Ownership festlegen | Verantwortlichkeiten zwischen WorkItemTracking, StationTracking, KpiCollector und SnapshotBuilder sind im Architekturdokument beschrieben; Implementierung der weiteren Komponenten steht noch aus |
| #005C | Offen | TrackingSubject-Registry modellieren | Technische Tracking-GUID von fachlicher Referenz trennen und Mindestmetadaten fuer Lookup mit GUID-basierter Fremdreferenz definieren |

---

## Hohe Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #010 | Offen | MVP-Prozess fachlich festzurren | Linearer Flow `Source -> Picking -> Packing -> Shipping` mit klaren Statuswechseln definieren | #001 |
| #010A | In Arbeit | Order-Statusuebergaenge definieren | `Created` als externer Einstieg sowie interne Queue-/Processing-Status fachlich abstimmen | #010 |
| #010B | Offen | WorkItem-Begriff im Modell verankern | Benamung fuer das durch den Prozess fliessende Objekt auf `WorkItem` umstellen und Auswirkungen auf Runtime/Snapshots pruefen | #001 |
| #011 | Offen | Simulationszustand modellieren | Mutable Runtime-Zustaende fuer WorkItems, Stationen und globale Zaehler abgrenzen | #003 |
| #011A | In Arbeit | GenerateSimulationEvent spezifizieren | Batch-Fenster, Erzeugungslogik, Abbruchbedingung und Folgeplanung des Generators definieren | #003 |
| #011B | In Arbeit | WorkItem-Tracking-Modell umstellen | Segmentbasiertes Tracking von `OrderTracking` auf generisches `WorkItemTracking` mit neutralen Status- und Methodennamen ueberfuehren | #005 |
| #011C | In Arbeit | Station-Tracking-Modell definieren | Tracking pro konkreter Station sowie Aggregation ueber konfigurierbare Stage-Definitionen fuer parallele Queues festlegen | #005 |
| #011D | In Arbeit | KpiCollectorState definieren | Kompakte Aggregatdaten, Trendpuffer und Bottleneck-Eingaben fuer KPI-Projektion festlegen | #005 |
| #011E | In Arbeit | Requeue-/OnHold-Tracking festlegen | Segment-Abschluss und Neustart fuer Queue, Processing, OnHold und Completion sind beschrieben; Rebalancing und Resume-Regeln muessen noch konkret umgesetzt werden | #011B |
| #011F | Offen | Stage-Konfiguration statt Enum modellieren | `ProcessStage` schrittweise durch konfigurierbare `StageDefinition`- und `StationDefinition`-Modelle ersetzen | #010 |
| #011G | Offen | JSON-Szenarioformat fuer Stages und Stations festlegen | Hierarchisches Dateiformat mit `stages -> stations`, Sequenzierung, externen Keys ohne IDs und Validierungsregeln verbindlich definieren | #011F |
| #011H | Offen | Domain ProcessConfiguration festlegen | Nach dem Laden GUIDs fuer Scenario/Stage/Station erzeugen und die validierte Konfiguration als Domain-Modell definieren, das direkt von der Simulation genutzt wird | #011F |
| #011I | Offen | Tracking an StageId und StationId anbinden | `WorkItemTracking`, `StationTracking` und spaetere Snapshots an konfigurierte IDs statt `ProcessStage` koppeln | #011F |
| #012 | In Arbeit | Event-Handler-Schnitt spezifizieren | Verantwortlichkeiten von Queue-Owner, `SimulationExecutionContext`, Runner, Dispatcher, Scheduler und Handlern sind im Architekturdokument beschrieben; Interface-Zuschnitt und Implementierung folgen | #003 |
| #013 | Offen | Snapshot-DTOs entwerfen | Station-, WorkItem-, KPI- und Alert-Sichten fuer Desktop und API vereinheitlichen | #004 |
| #014 | Erledigt | Delivery-Strategie festgelegt | Desktop-first ist gesetzt, eine API wird frueh aufgebaut und soll moeglichst dieselben Kernschnittstellen nutzen | -- |
| #015 | Offen | Persistenzstrategie fuer Szenarien waehlen | JSON-Dateien fuer MVP bestaetigen oder frueh eine DB-Option vorbereiten | -- |
| #015A | Offen | Szenario-Dateiablage festlegen | Verzeichnisstruktur wie `scenarios/*.json`, Dateinamenregeln und Schema-Versionierung fuer den MVP festlegen; Dateien enthalten nur externe Keys, keine IDs | #015 |
| #015B | Offen | Checkpoint-Dateiformat konkretisieren | Portable Ein-Datei-Struktur `*.flowforge-run.json`, Versionierung, JSON-Konverter und minimale technische Validierung fuer Resume/Sharing festlegen | #004E |
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
| #044 | In Arbeit | Event-Prioritaetsmodell | Reihenfolge wie `Completed` vor `Queued` vor `Started` vor `Generate` verbindlich festlegen |
| #045 | In Arbeit | Run-/Versionsinvalidation | Festlegen, wie veraltete Events ueber `ProcessingToken` oder aehnliche Marker erkannt und geskippt werden |
| #046 | In Arbeit | KPI-Platzierung im Snapshot | Festlegen, welche KPIs inkrementell berechnet und als Teil des publizierten Snapshots gespeichert werden |
| #047 | In Arbeit | Snapshot-Retention | Umfang der Run-Timeline, Playback-Bedarf und spaetere Kompaktierungsstrategie festlegen |
| #048 | In Arbeit | KPI-Ausloeser definieren | Festlegen, welche Event-Handler Fakten/Aggregate aktualisieren und wie `SnapshotPublishedEvent` die KPI-Projektion ausloest |
| #049 | Offen | Segment-Aggregationsregeln | Definieren, wie Queue-, Processing- und Hold-Zeiten ueber Stationen und Stages summiert werden |
| #049A | In Arbeit | Tracking-Service-Haertung | Result-Semantik, Nullability und konsistente Fehlerbehandlung fuer den Tracking-Service weiter schaerfen |
| #049B | Offen | TrackingSubject-Referenzierungsmodell festlegen | Definieren, welche Felder eine Registry fuer `TrackingSubjectId -> EntityRef` im MVP mindestens enthalten muss und wie GUID-basierte Fremdreferenzen gespeichert werden |
| #049C | Offen | Konfigurationsvalidierung festlegen | Regeln fuer eindeutige externe Stage-/Station-Keys, Stage-Reihenfolge, Pflichtfelder und referenzielle Konsistenz definieren und der Simulation als Owner zuordnen |
| #049D | Offen | Import-ID-Generierung festlegen | Strategie fuer GUID-Erzeugung, Key-zu-ID-Mapping und Fehlerverhalten bei doppelten externen Keys definieren |

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
| #001B | Erledigt | Simulation-Projekt angelegt | `FlowForge.Simulation` wurde in der Solution angelegt und mit der Zielstruktur vorbereitet |
| #001C | Erledigt | OrderTracking als Runtime-Aggregat umgesetzt | `OrderTracking`, `OrderTrackingSegment` und `OrderTrackingService` wurden auf kontrollierte Mutabilitaet mit read-only Segmentzugriff ausgerichtet |
| #001D | Erledigt | WorkItem als bevorzugte neutrale Benamung gewaehlt | Fuer das generisch trackbare Objekt im Simulationsfluss wird kuenftig `WorkItem` als bevorzugter Begriff verwendet |
| #001E | Erledigt | Checkpoint-Vertrag neu verortet | Checkpoint-Dokumente liegen im `Simulation`-Layer, das Speicher-Interface im `Application`-Layer, die JSON-Implementierung in `Infrastructure` |
