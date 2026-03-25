# Vision -- FlowForge

Arbeitsdokument fuer mittelfristige Vorhaben, spaetere Entscheidungen und groessere Zielbilder.
Dieses Dokument enthaelt bewusst keine unmittelbaren Umsetzungstickets, sondern Themen, die noch nicht als naechster Arbeitsschritt in `docs/Todo.md` liegen.

## Nutzungsregeln

- Ein Thema gehoert hierher, wenn es wichtig ist, aber noch nicht direkt umgesetzt werden soll.
- Ein Thema sollte trotzdem konkret beschrieben sein: Zielbild, Nutzen, moeglicher Ausloeser und betroffene Dokumente.
- Sobald die naechste konkrete Arbeit klar ist, wird daraus ein Eintrag in `docs/Todo.md`.

## Produktnahe Erweiterungen

### Stoerungsmodell fuer Erweiterungsinkrement

- Zielbild: Ausfaelle, Shipping-Stopp, Requeue aufgrund externer Ereignisse oder Lastspitzen als explizite Simulationsereignisse modellieren.
- Nutzen: realistischere Engpass- und Resilienzsimulation.
- Ausloeser fuer Umsetzung: sobald der normale Happy-Path der Simulation stabil laeuft.
- Relevante Dokumente: `docs/architecture/design/simulation-events.md`, `docs/architecture/design/simulation-dispatching.md`, `docs/architecture/design/simulation-orchestration.md`

### Replay- und Playback-Ausbau

- Zielbild: gespeicherte oder wiederabspielbare Simulationslaeufe fuer Analyse, Demos und spaetere Exporte.
- Nutzen: bessere Nachvollziehbarkeit von Runs und staerkere Desktop-Demo.
- Ausloeser fuer Umsetzung: sobald Snapshot-Timeline und Checkpoint-Basis produktiv nutzbar sind.
- Relevante Dokumente: `docs/architecture/design/snapshots-and-kpis.md`, `docs/architecture/design/checkpoints.md`

### Demo-Szenarien und Engpass-Szenarien

- Zielbild: kuratierte Referenzszenarien fuer normale Auslastung, Engpassbildung und spaetere Stoerungen.
- Nutzen: bessere Produktkommunikation, Tests und Demo-Faehigkeit.
- Ausloeser fuer Umsetzung: sobald Scenario-Import und Snapshot-Ausgabe stabil sind.
- Relevante Dokumente: `docs/architecture/design/scenario-configuration.md`

## Zukuenftige Architekturentscheidungen

### Persistenzrichtung nach dem JSON-MVP

- Zielbild: entscheiden, ob nach dem MVP eine relationale oder andere persistente Speicherung fuer Szenarien, Runs oder Auswertungen sinnvoll ist.
- Nutzen: vorbereitetes Wachstum ueber lokale JSON-Dateien hinaus.
- Ausloeser fuer Entscheidung: wenn Szenarioverwaltung, Mehrbenutzerfaehigkeit oder laengere Run-Historien relevant werden.
- Relevante Dokumente: `docs/architecture/components/infrastructure.md`, `docs/architecture/design/checkpoints.md`

### Snapshot-Publikationsstrategie verfeinern

- Zielbild: entscheiden, ob feste Intervalle, ereignisgetriebene Publikation oder ein Hybridmodell langfristig besser passen.
- Nutzen: bessere Balance zwischen Korrektheit, Performance und UI-Erlebnis.
- Ausloeser fuer Entscheidung: wenn die erste Snapshot-Pipeline laeuft und gemessen werden kann.
- Relevante Dokumente: `docs/architecture/design/snapshots-and-kpis.md`

### Vollstaendige Umstellung von `ProcessStage` auf konfigurierte IDs

- Zielbild: Stage- und Station-Referenzen im Runtime- und Snapshot-Modell vollstaendig ueber konfigurierte IDs fuehren.
- Nutzen: frei konfigurierbare Topologien ohne harte Enum-Kopplung.
- Ausloeser fuer Umsetzung: sobald die erste Domain-`ProcessConfiguration` und der Scenario-Import stabil eingefuehrt sind.
- Relevante Dokumente: `docs/architecture/design/scenario-configuration.md`, `docs/architecture/design/simulation-events.md`

## Delivery- und Produktvision

### Desktop als primärer Simulationsclient

- Zielbild: visuelle Prozessdarstellung, KPI-Dashboard, Playback-Timeline und interaktive Simulationssteuerung in einem dedizierten Desktop-Host.
- Nutzen: starkes MVP und spaetere Demo- und Analyseoberflaeche.
- Ausloeser fuer Umsetzung: sobald Snapshot-Contracts, Runtime-Basis und zentrale Use-Cases stehen.
- Relevante Dokumente: `docs/architecture/components/delivery.md`, `docs/architecture/design/application-contracts.md`

### Fruehe API mit gemeinsamem Kernmodell

- Zielbild: API fuer Control- und Query-Szenarien, die dieselben Kern-Contracts wie Desktop und CLI nutzt.
- Nutzen: bessere Automatisierung, spaetere Remote-Szenarien und geringere Modell-Divergenz.
- Ausloeser fuer Umsetzung: sobald Application-Use-Cases und Snapshot-DTOs stabil sind.
- Relevante Dokumente: `docs/architecture/components/delivery.md`, `docs/architecture/design/application-contracts.md`

## Spaetere Erweiterungsrichtungen

### Export- und Reporting-Faehigkeiten

- Zielbild: Export von KPI-Reihen, Snapshot-Timelines oder Run-Zusammenfassungen fuer Analyse und Praesentation.
- Nutzen: besserer fachlicher Wert ausserhalb der eigentlichen Live-Simulation.
- Ausloeser fuer Umsetzung: sobald Snapshot- und Checkpoint-Modell stabil sind.
- Relevante Dokumente: `docs/architecture/components/infrastructure.md`, `docs/architecture/design/snapshots-and-kpis.md`, `docs/architecture/design/checkpoints.md`

### Erweiterbare Flussmodelle

- Zielbild: spaetere Unterstuetzung fuer Rework, Branching oder priorisierte WorkItems.
- Nutzen: Oeffnung ueber den linearen MVP-Fulfillment-Flow hinaus.
- Ausloeser fuer Umsetzung: erst nach stabilem linearem Grundmodell.
- Relevante Dokumente: `docs/architecture/design/scenario-configuration.md`, `docs/architecture/design/simulation-orchestration.md`
