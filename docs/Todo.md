# TODO-Liste -- FlowForge

Arbeitsdokument fuer konkrete naechste Aufgaben.
Dieses Dokument enthaelt nur Aufgaben, die direkt umgesetzt, vorbereitet oder abgeschlossen werden koennen.

Langfristige Vorhaben, spaetere Entscheidungen und groessere Zielbilder liegen in `docs/Vision.md`.
Abgeschlossene Aufgaben werden aus dieser Datei nach `docs/DoneTasks.md` verschoben.

**Legende Status:** `Offen` · `In Arbeit` · `Blockiert` · `Erledigt`

## Priorisierung

- Ein Eintrag gehoert nur in diese Datei, wenn die naechste konkrete Arbeit klar ist.
- Jeder Eintrag sollte auf ein relevantes Design-Dokument verweisen, wenn die Umsetzung davon abhaengt.
- Abstrakte Zielbilder, groessere Optionen und spaetere Produktentscheidungen gehoeren nicht hierher.

## Jetzt als naechstes umsetzbar

| ID | Status | Aufgabe | Konkreter naechster Schritt | Referenz |
|---|---|---|---|---|
| T001 | Offen | Domain-`ProcessConfiguration` im Domain-Layer anlegen | `ProcessConfiguration`, `ArrivalProfileDefinition`, `StageDefinition` und `StationDefinition` in `FlowForge.Domain` einfuehren und Grundinvarianten mit modellieren | `docs/architecture/design/scenario-configuration.md` |
| T003 | Offen | Szenario-Import in Domain-Mapping ueberfuehren | Externe Stage-/Station-Keys in GUID-basierte Domain-Konfiguration normalisieren und Mapping-Regeln kapseln | `docs/architecture/design/scenario-configuration.md` |
| T035 | Offen | Fehlende Runtime-Handler fuer Hold, Resume, Stop, Requeue und Cancel ergaenzen | Eventtypen, Handlerregistrierung und Orchestrator-Anbindung fuer die noch offenen Laufzeitpfade sauber vertikal durchziehen | `docs/architecture/design/simulation-events.md`, `docs/architecture/design/simulation-dispatching.md`, `docs/architecture/design/simulation-orchestration.md` |
| T036 | Offen | Offene Orchestrator- und Runtime-Uebergaenge fuer Cancel, Resume und Disruptionen vervollstaendigen | Fehlende Commands, Handlerpfade, Runtime-Mutationen und Tracking-Anbindung fuer die noch nicht durchgaengig implementierten Uebergaenge abschliessen | `docs/architecture/design/simulation-orchestration.md`, `docs/architecture/design/simulation-events.md` |
| T012 | Offen | Ersten `ProcessingComplete`-Pfad vertikal umsetzen | Handler, Orchestrator-Aufruf, Tracking-Update, KPI-Fact-Update und Follow-up-Scheduling fuer Completion als ersten End-to-End-Pfad implementieren | `docs/architecture/design/simulation-orchestration.md`, `docs/architecture/design/simulation-events.md` |

## Kurzfristig danach

| ID | Status | Aufgabe | Konkreter naechster Schritt | Referenz |
|---|---|---|---|---|
| T002 | Offen | JSON-Szenarioformat als Persistence-Modell umsetzen | Wieder aufnehmen, sobald der In-Memory-Runtime-Slice stabil ist; dann JSON-Modelle und Validierung fuer `scenarioKey`, `arrivalProfile`, `stages` und `stations` in `FlowForge.Infrastructure` anlegen | `docs/architecture/design/scenario-configuration.md`, `docs/Vision.md` |
| T020 | Offen | Snapshot-Root-DTOs anlegen | `SimulationSnapshot`, `ProcessSnapshot`, `StationSnapshot`, `WorkItemSnapshot`, `KpiSnapshot` und `SnapshotMetadata` als erste Contracts definieren | `docs/architecture/design/snapshots-and-kpis.md` |
| T021 | Offen | Latest-Snapshot-Store und Timeline-Store einfuehren | Atomaren Latest-Snapshot-Swap und einfache In-Memory-Timeline fuer einen Run implementieren | `docs/architecture/design/snapshots-and-kpis.md` |
| T022 | Offen | `KpiCollectorState` als kompakte Aggregatbasis einfuehren | Kernfelder fuer Created, Completed, Lead Time, WIP und Trend-Buffer als Runtime-Komponente definieren | `docs/architecture/design/snapshots-and-kpis.md` |
| T023 | Offen | Checkpoint-Basisvertraege anlegen | `SimulationExecutionState`, `SimulationCheckpointDocument` und die wichtigsten Subdokumente in `FlowForge.Simulation` einfuehren | `docs/architecture/design/checkpoints.md` |
| T024 | Offen | Checkpoint-Store-Port in Application anlegen | `ISimulationCheckpointStore` als Application-Port definieren und fuer spaetere Infrastructure-Implementierung vorbereiten | `docs/architecture/design/checkpoints.md` |
| T025 | Offen | Simulation-Testsprojekt anlegen | `tests/FlowForge.Simulation.Tests` erstellen und erste Tests fuer Event-Ordering und Runner-Verhalten hinzufuegen | `docs/architecture/design/simulation-runner.md`, `docs/architecture/design/simulation-events.md` |
| T033 | Offen | `FlowForge.UiWpf` an echte Snapshot- und Control-Use-Cases anbinden | Application-Ports fuer Start, Pause, Reset und Latest-Snapshot-Read konkretisieren und die WPF-Ansicht statt Sample-Daten daran anbinden | `docs/architecture/components/delivery.md`, `docs/architecture/design/application-contracts.md`, `docs/architecture/design/snapshots-and-kpis.md` |

## Vorbereitet, aber aktuell blockiert

| ID | Status | Aufgabe | Blockiert durch | Referenz |
|---|---|---|---|---|
| T030 | Blockiert | Application-Use-Cases fuer Start, Pause und Snapshot-Query konkret anlegen | Benoetigt lauffaehige Simulation-Basis und Snapshot-Contracts | `docs/architecture/design/application-contracts.md` |
| T031 | Blockiert | API-Endpunkte fuer Simulation-Control und Snapshot-Read modellieren | Benoetigt Application-Use-Cases und gemeinsame DTOs | `docs/architecture/components/delivery.md`, `docs/architecture/design/application-contracts.md` |
| T032 | Blockiert | CLI-Debug-Workflows auf neue Simulation-Basis aufsetzen | Benoetigt Runner, Events und mindestens einen vertikalen Runtime-Pfad | `docs/architecture/components/delivery.md`, `docs/architecture/design/simulation-runner.md` |
