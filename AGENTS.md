# AGENTS.md

Dette er repoets korte instruksfil for agenter. Den skal ikke beskrive den agentiske utviklingsworkflowen.

## Obligatorisk Workflow

Alle agenter skal lese og følge `docs/workflows/agentic-development.md` før de gjør arbeid i dette repoet.

`docs/workflows/agentic-development.md` er eneste autoritative kilde for agentisk utviklingsworkflow. Ikke dupliser, forkort eller erstatt workflowen i denne filen.

Hvis `docs/workflows/agentic-development.md` mangler eller ikke kan leses, skal arbeidet stoppes og blokkeringen rapporteres før kode, dokumentasjon eller prosjektstruktur endres.

## Prosjektkontekst

Lightswitch er en liten Windows desktop utility for Logitech Litra Glow.

Detaljerte produktkrav, arkitekturkrav, beslutninger og arbeidsnotater skal ligge i dedikerte dokumenter, ikke i `AGENTS.md`.

## Teknologi

- C#
- .NET 8 eller .NET 9
- WPF app under `src/Lightswitch.Wpf`
- `System.Windows.Forms.NotifyIcon` for WPF tray icon
- HidSharp for direkte USB HID-kommunikasjon

## Lokalt Miljø

- Standard shell er PowerShell på Windows.
- Kommandoeksempler for dette repoet bør antas å kjøres fra PowerShell med mindre noe annet er spesifisert.
