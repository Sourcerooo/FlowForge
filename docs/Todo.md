# TODO-Liste -- FlowForge

Arbeitsdokument fuer konkrete Aufgaben, naechste Schritte und technische Nacharbeiten.
Dieses Dokument ist bewusst operativer als `docs/Roadmap.md` und sollte regelmaessig gepflegt werden.

**Legende Status:** `Offen` · `In Arbeit` · `Blockiert` · `Erledigt`

## Inhaltsverzeichnis

- [Naechste Schritte](#naechste-schritte)
- [Hohe Prioritaet](#hohe-prioritaet)
- [Mittlere Prioritaet](#mittlere-prioritaet)
- [Niedrige Prioritaet](#niedrige-prioritaet)
- [Technische Schulden](#technische-schulden)
- [Erledigt](#erledigt)

---

## Naechste Schritte

| ID | Status | Aufgabe | Beschreibung |
|---|---|---|---|
| #001 | Offen | Erste Fachlichkeit definieren | Kernbegriffe, Regeln und Ziele der ersten Domäne beschreiben |
| #002 | Offen | Ersten Use Case umsetzen | Einen kleinen, durchgaengigen Anwendungsfall von API oder CLI bis Domain bauen |
| #003 | Offen | Dependency Injection verdrahten | Reale Registrierungen in `Application` und `Infrastructure` ergaenzen |
| #004 | Offen | Testbasis erweitern | Domain- und Application-Tests fuer den ersten Use Case anlegen |
| #005 | Offen | Dokumentation konkretisieren | README und Architekturdokument auf das echte Projekt zuschneiden |

---

## Hohe Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #010 | Offen | Fehlerbehandlung festlegen | Einheitliches Result-/Exception-Konzept fuer API und CLI definieren | -- |
| #011 | Offen | Persistenzstrategie waehlen | Datenhaltung, Repository-Schnittstellen und Migrationsstrategie festlegen | #001 |
| #012 | Offen | Konfiguration strukturieren | Einstellungen pro Umgebung und Secrets-Handling definieren | -- |
| #013 | Offen | Logging-Konzept erstellen | Strukturierte Logs, Kategorien und Betriebsdiagnostik festlegen | -- |
| #014 | Offen | Erste Integrationsgrenze modellieren | Externe Systeme ueber Application-Interfaces kapseln | #002 |

---

## Mittlere Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #020 | Offen | CLI-Befehle erweitern | Konkrete Commands fuer Betrieb, Import oder Wartung einfuehren | #002 |
| #021 | Offen | API-Kontrakte schaerfen | Request-/Response-Modelle, Validierung und Fehlermapping ausbauen | #002 |
| #022 | Offen | Docker-Setup erweitern | Healthchecks, Volumes und Umgebungsvariablen ergaenzen | #012 |
| #023 | Offen | CI absichern | Caching, Qualitaetschecks und ggf. Artefakte in GitHub Actions ergaenzen | -- |
| #024 | Offen | Integrationstests anlegen | Separates Testprojekt fuer Infrastruktur oder API pruefen | #011 |

---

## Niedrige Prioritaet

| ID | Status | Aufgabe | Beschreibung | Abhaengigkeit |
|---|---|---|---|---|
| #030 | Offen | Developer Experience verbessern | Skripte, Makefile oder Wrapper fuer haeufige Kommandos bereitstellen | -- |
| #031 | Offen | Beispielkonfigurationen anlegen | Vorlagen fuer lokale Entwicklung und Deployment dokumentieren | #012 |
| #032 | Offen | Architekturentscheidungen protokollieren | Wichtige Entscheidungen als ADR oder Entscheidungstabelle festhalten | -- |

---

## Technische Schulden

| ID | Status | Thema | Beschreibung |
|---|---|---|---|
| #040 | Offen | Platzhaltercode entfernen | Starter-Code in API, CLI und Tests schrittweise durch reale Implementierungen ersetzen |
| #041 | Offen | Abhaengigkeiten ueberpruefen | Pakete regelmaessig validieren und nicht benoetigte References entfernen |
| #042 | Offen | Dokumentation synchron halten | Architektur- und Setup-Dokumente bei Struktur-Aenderungen sofort nachziehen |

---

## Erledigt

| ID | Status | Aufgabe | Beschreibung |
|---|---|---|---|
| #000 | Erledigt | Projektgeruest erzeugt | Clean-Architecture-Solution mit API, CLI, Tests, Docker und CI angelegt |
