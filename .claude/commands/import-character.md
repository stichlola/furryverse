---
description: Pipeline Blender → Unity per un personaggio (argomento: nome del personaggio)
argument-hint: <NomePersonaggio>
---

Importa il personaggio **$ARGUMENTS** seguendo `docs/ART_PIPELINE.md`. Chiedi l'ok a Davide prima di modificare il modello.

## Blender (Blender MCP)
1. Screenshot del viewport e riepilogo della scena: mesh, armature, action presenti.
2. Controlli: scala (altezza 1.6-1.9 m), origine ai piedi, trasformazioni applicate, normali, doppi, tris (15-40k).
3. Rig: scheletro umanoide + catene extra (`Tail_*`, `Ear_L_*`, `Ear_R_*`, ...). Segnala ossa mancanti o nomi non standard.
4. Animazioni: verifica le clip della tabella in `ART_PIPELINE.md`; elenca quelle mancanti.
5. Export FBX con le impostazioni della sezione 4 in `Assets/_Project/Art/Characters/$ARGUMENTS/$ARGUMENTS.fbx`. Screenshot finale.

## Unity (Unity MCP)
6. Import: Rig **Humanoid**, controlla l'Avatar; clip con loop come da tabella, Root Transform bloccato.
7. Animator Controller in `Assets/_Project/Animations/$ARGUMENTS/` con i parametri `Speed`, `VelocityY`, `Grounded`, `WallSlide`, trigger `Jump`, `Land`, `Dash`.
8. Nel prefab `Player`: sostituisci la capsula in `Visual` con il modello, assegna Animator e `PlayerVisuals.animator`.
9. Aggiungi `SpringBoneChain` alla radice di coda e di ogni orecchio (`bonesAreAnimated = false`).
10. Controlla la Console, prova in Play mode nella scena di test e riporta l'esito.

Alla fine spunta le voci corrispondenti in `docs/ROADMAP.md`.
