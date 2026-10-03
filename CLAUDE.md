# CLAUDE.md — Platform furry 2.5D (nome provvisorio)

Istruzioni per Claude Code. Leggile all'inizio di ogni sessione.

## Il progetto
Platform 2.5D artistico in stile *Ori and the Blind Forest*: personaggi furry (animali antropomorfi, tutti adulti), mondo dipinto e luminoso, movimento fluido e preciso. Sviluppatore singolo (Davide), lavora part-time.
Documenti di riferimento: `docs/GDD.md` (design), `docs/ROADMAP.md` (fasi e task), `docs/ART_PIPELINE.md` (Blender → Unity).

## Lingua e stile di lavoro
- Rispondi in **italiano**, breve e diretto. Output pratici, niente spiegazioni lunghe.
- Commenti nel codice in italiano, nomi di classi/variabili in inglese.
- Prima di modifiche grandi, proponi un piano in 3-5 punti e aspetta l'ok.
- Dopo ogni task completato, spunta la voce in `docs/ROADMAP.md`.

## Stack
- Unity 6 LTS, URP, progetto 3D usato come 2.5D (gameplay sul piano X/Y, Z = profondità scenica).
- Input System (nuovo), Cinemachine, Animation Rigging, ProBuilder per il grey-box.
- Blender per modelli, rig e animazioni. Modelli base generati con Tripo e rifiniti in Blender.
- MCP: `unityMCP` (MCP for Unity, CoplayDev) e `blender` (blender-mcp). Configurazione in `.mcp.json`.

## Regole tecniche
- Script in `Assets/_Project/Scripts/`, assembly `Project.Runtime`. Namespace `Project.<Area>`.
- Asset del progetto solo dentro `Assets/_Project/` (Art, Animations, Prefabs, Scenes, Materials, VFX, Audio).
- Movimento: controller **cinematico** (`PlayerMotor`, CharacterController). La fisica Rigidbody solo per oggetti ed effetti.
- Asse Z del player sempre a 0. Livelli: piattaforme collidibili su Z = 0, decorazioni a Z ≠ 0 (davanti e dietro) per la profondità.
- Animazione "economica ma bella": poche clip, il resto procedurale (`PlayerVisuals` per squash/stretch e inclinazione, `SpringBoneChain` per coda, orecchie, capelli, vestiti).
- Valori di tuning esposti nell'Inspector con `[Header]` e default sensati. Niente numeri magici nel codice.
- Mai modificare file in `Library/`, `Temp/`, `Logs/`.

## Uso degli MCP
- **Unity MCP:** per creare scene, GameObject, prefab, materiali e leggere la Console. Dopo aver scritto script, controlla la Console per errori di compilazione prima di dire "fatto".
- **Blender MCP:** per pulizia mesh, rig, retarget, export FBX. Segui `docs/ART_PIPELINE.md`. Fai uno screenshot del viewport per verificare il risultato.
- Workflow modelli: Claude elenca i modelli da generare → Davide li genera su Tripo Studio e li importa → Claude li sistema solo dopo l'ok di Davide.

## Contenuti
- Tutti i personaggi sono **chiaramente adulti** nel design e nella scrittura. Nessuna eccezione.
- Il tono può essere ammiccante/suggestivo (outfit, pose, dialoghi), **mai esplicito**. Niente temi tabù o non consensuali: il gioco deve passare la revisione di Steam.
- Il livello di "edgy" è ancora da decidere (vedi `docs/GDD.md`): non introdurre contenuti suggestivi finché non è deciso.
