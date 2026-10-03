# Platform furry 2.5D — setup

Progetto di partenza per Claude Code: documenti, istruzioni per Claude, MCP già configurati e i primi script di movimento.

## 1. Prerequisiti
- **Unity Hub** + **Unity 6 LTS** (modulo Windows Build Support).
- **Blender** 4.x con l'add-on **blender-mcp** attivo (pannello BlenderMCP → *Connect*).
- **uv** (serve per i server MCP): https://docs.astral.sh/uv/
- **Git** + **Git LFS**.
- **Claude Code** installato (`claude --version`).

## 2. Crea il progetto Unity
1. Unity Hub → *New project* → template **Universal 3D** → come cartella scegli una cartella vuota, es. `PlatformFurry_Unity`.
2. Chiudi Unity. Copia **dentro** quella cartella tutto il contenuto di questo pacchetto (`Assets/`, `docs/`, `.claude/`, `CLAUDE.md`, `.mcp.json`, `.gitignore`, `.gitattributes`). Accetta l'unione con la cartella `Assets` esistente.
3. Riapri il progetto. In *Window → Package Manager* installa:
   - **Input System** (se chiede di riavviare e attivare il nuovo backend: Sì)
   - **Cinemachine**
   - **Animation Rigging**
   - **ProBuilder**
   - **MCP for Unity**: `+` → *Add package from git URL* → `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`
4. *Edit → Project Settings → Player → Active Input Handling* = **Input System Package (New)** (o *Both*).
5. *Window → MCP for Unity* → **Start Server** (indicatore verde). In alternativa usa *Auto-Setup* → Claude Code.

## 3. Git
```bash
git init
git lfs install
git add .
git commit -m "Setup iniziale"
```

## 4. Avvia Claude Code
Dalla cartella del progetto, con Unity e Blender aperti:
```bash
claude
```
Approva i server MCP del progetto quando te lo chiede, poi verifica con `/mcp` che `unityMCP` e `blender` siano connessi.

Primo comando consigliato:
```
/setup-test-scene
```
Crea una scena di prova con il player (capsula) e un livello grey-box per testare il movimento.

## Comandi inclusi
- `/setup-test-scene` — scena di test del movimento.
- `/import-character` — pipeline Blender → Unity per un personaggio.

## Controlli (default)
- Movimento: A/D o frecce, stick sinistro
- Salto: Spazio, A (gamepad) — tieni premuto per saltare più in alto
- Dash: Shift sinistro, grilletto destro
- Doppio salto e salto a muro inclusi

## Se un MCP non si collega
- Unity: controlla che il server sia avviato in *Window → MCP for Unity*; la porta di default è 8080. Se usi una porta diversa, cambiala in `.mcp.json`.
- Blender: l'add-on deve essere attivo e connesso; serve `uvx` nel PATH.
- Se l'Auto-Setup di Unity ha già registrato il server in Claude Code, rimuovi la voce `unityMCP` da `.mcp.json` per evitare doppioni.
