# Abgeschlossene Aufgaben -- FlowForge

Dieses Dokument enthaelt alle abgeschlossenen Aufgaben, die aus `docs/Todo.md` verschoben wurden.
Sobald ein Task in der Todo-Liste abgeschlossen ist, wird er aus `docs/Todo.md` entfernt und hier eingetragen.

## Abgeschlossen

| ID | Status | Aufgabe | Ergebnis | Referenz |
|---|---|---|---|---|
| T005 | Erledigt | Queue- und Scheduler-Schnittstellen anlegen | `ISimulationEventQueue`, `ISimulationEventScheduler` sowie eine priorisierte Queue mit zentraler Sortierung sind angelegt; die run-scoped Einbindung wird ueber T004 weiter verfeinert | `docs/architecture/design/simulation-runner.md`, `docs/architecture/design/simulation-events.md` |
| T006 | Erledigt | Basis fuer Simulationsevents definieren | `SimulationEvent`, `EventKind`, `EventSortRank` und erste Eventtypen fuer Generate, Queue, Start, Complete und Snapshot sind angelegt | `docs/architecture/design/simulation-events.md` |
| T007 | Erledigt | Dispatcher- und Registry-Basis anlegen | Dispatcher-Grundstruktur und erste Handler-Aufloesung sind angelegt; die spaetere Aufloesung wurde inzwischen auf DI-basierte Handler-Komposition umgestellt | `docs/architecture/design/simulation-dispatching.md` |
| T008 | Erledigt | `SimulationRunner` implementieren | Dequeue-Loop, Zeitfortschritt und Dispatch-Aufruf mit CancellationToken sind in `FlowForge.Simulation` vorhanden | `docs/architecture/design/simulation-runner.md` |
| T004 | Erledigt | `SimulationExecutionContext` und `SimulationExecutionHandlerContext` anpassen | `SimulationExecutionContext` wurde auf run-scoped Ausfuehrungsdaten reduziert; Dispatcher, Scheduler und Handler werden ueber DI an die beteiligten Runtime-Komponenten gegeben | `docs/architecture/design/simulation-execution-context.md`, `docs/architecture/design/simulation-runner.md` |
| T009 | Erledigt | `WorkItemTracking` auf Segmentmodell umstellen | `WorkItemTracking` fuehrt Segmenthistorie mit `TrackingSegmentType` und `ProcessingToken`; die Semantik unterscheidet zwischen Stage-Abschluss und finalem WorkItem-Abschluss | `docs/architecture/design/scenario-configuration.md` |
| T010 | Erledigt | `StationTracking` und `StageTracking` einfuehren | Stage- und Station-Tracking sind als run-scoped Aggregationen vorhanden; `StageTrackingStore` initialisiert Stage-Trackings pro Prozesskonfiguration und delegiert stationaere Kennzahlen an `StationTracking` | `docs/architecture/design/scenario-configuration.md` |
| T011 | Erledigt | `IWorkItemProcessOrchestrator` und Commands anlegen | Der Orchestrator sowie die benoetigten Command-Typen fuer die aktuellen Runtime-Pfade sind vorhanden; weitere Uebergaenge laufen als separate Folgeaufgaben | `docs/architecture/design/simulation-orchestration.md` |
| T013 | Erledigt | Registrierung von Handler-Instanzen umsetzen | Handler werden ueber Dependency Injection registriert und vom Dispatcher implizit ueber `IEnumerable<ISimulationEventHandler>` und `CanHandle()` aufgeloest | `docs/architecture/design/simulation-dispatching.md`, `docs/architecture/design/simulation-execution-context.md` |
| T034 | Erledigt | WPF-Desktop-Projekt und Simulationsoberflaeche anlegen | `FlowForge.UiWpf` ist in der Solution eingebunden und stellt eine mockup-orientierte Simulation-Ansicht mit Netzwerk-Canvas, KPI-Panels, Trends, Alerts und Timeline dar | `docs/architecture/components/delivery.md` |
