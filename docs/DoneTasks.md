# Abgeschlossene Aufgaben -- FlowForge

Dieses Dokument enthaelt alle abgeschlossenen Aufgaben, die aus `docs/Todo.md` verschoben wurden.
Sobald ein Task in der Todo-Liste abgeschlossen ist, wird er aus `docs/Todo.md` entfernt und hier eingetragen.

## Abgeschlossen

| ID | Status | Aufgabe | Ergebnis | Referenz |
|---|---|---|---|---|
| T005 | Erledigt | Queue- und Scheduler-Schnittstellen anlegen | `ISimulationEventQueue`, `ISimulationEventScheduler` sowie eine priorisierte Queue mit zentraler Sortierung sind angelegt; die run-scoped Einbindung wird ueber T004 weiter verfeinert | `docs/architecture/design/simulation-runner.md`, `docs/architecture/design/simulation-events.md` |
| T006 | Erledigt | Basis fuer Simulationsevents definieren | `SimulationEvent`, `EventKind`, `EventSortRank` und erste Eventtypen fuer Generate, Queue, Start, Complete und Snapshot sind angelegt | `docs/architecture/design/simulation-events.md` |
| T007 | Erledigt | Dispatcher- und Registry-Basis anlegen | `EventRoutingKey`, `IEventDispatcher`, `IEventHandlerRegistry` und eine erste Lookup-Basis sind angelegt; die DI-basierte Handler-Registrierung laeuft jetzt als T013 weiter | `docs/architecture/design/simulation-dispatching.md` |
| T008 | Erledigt | `SimulationRunner` implementieren | Dequeue-Loop, Zeitfortschritt und Dispatch-Aufruf mit CancellationToken sind in `FlowForge.Simulation` vorhanden | `docs/architecture/design/simulation-runner.md` |
| T034 | Erledigt | WPF-Desktop-Projekt und Simulationsoberflaeche anlegen | `FlowForge.UiWpf` ist in der Solution eingebunden und stellt eine mockup-orientierte Simulation-Ansicht mit Netzwerk-Canvas, KPI-Panels, Trends, Alerts und Timeline dar | `docs/architecture/components/delivery.md` |
