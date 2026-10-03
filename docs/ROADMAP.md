# Roadmap verso la demo

Tempi stimati part-time. Spunta le voci man mano.

## Fase 0 — Setup (1 settimana)
- [ ] Progetto Unity 6 LTS (URP) creato e pacchetto copiato dentro
- [ ] Pacchetti: Input System, Cinemachine, Animation Rigging, ProBuilder, MCP for Unity
- [ ] Git + LFS, primo commit
- [ ] Claude Code collegato a Unity MCP e Blender MCP (`/mcp`)
- [ ] `/setup-test-scene`: capsula che corre, salta, fa dash e salto a muro

## Fase 1 — Concept (1 settimana)
- [ ] Protagonista: specie, personalità, outfit (`docs/GDD.md`)
- [ ] Meccanica distintiva scelta
- [ ] Tono deciso (pulito o suggestivo)
- [ ] Mood board del primo bioma (5-10 immagini)
- ✅ Checkpoint: GDD compilato nelle sezioni "DA DECIDERE"

## Fase 2 — Feel del movimento (1-2 settimane)
- [ ] Tuning di `PlayerMotor` (velocità, altezza salto, gravità in caduta)
- [ ] Camera Cinemachine con look-ahead e dead zone
- [ ] Juice: polvere a salto/atterraggio, scia del dash, camera shake leggero
- [ ] Meccanica distintiva prototipata con la capsula
- ✅ Checkpoint: muoversi è divertente anche in grey-box. **Inizia a postare GIF ogni settimana.**

## Fase 3 — Personaggio (2-3 settimane)
- [ ] Modello generato su Tripo e pulito in Blender
- [ ] Rig umanoide + ossa extra (coda, orecchie, eventuali vestiti)
- [ ] ~10 animazioni: idle, corsa, salto su/apice/giù, atterraggio, dash, muro, colpo, morte
- [ ] Import in Unity (`/import-character`), Animator collegato a `PlayerVisuals`
- [ ] `SpringBoneChain` su coda e orecchie
- ✅ Checkpoint: il personaggio vero sostituisce la capsula e sembra vivo

## Fase 4 — Look "Ori" (2 settimane)
- [ ] Post-processing URP: bloom, color grading, vignette, profondità con nebbia
- [ ] Shader Graph: rim light sul personaggio, materiali dipinti per l'ambiente
- [ ] Strati di profondità: primo piano scuro, piano di gioco, sfondo, cielo
- [ ] Particelle: lucciole, polline, raggi di luce
- ✅ Checkpoint: uno screenshot che sembra una cartolina

## Fase 5 — Vertical slice (2-3 settimane)
- [ ] Kit modulare del bioma (piattaforme, rocce, vegetazione)
- [ ] 1 livello completo: checkpoint, morte/respawn, collezionabili
- [ ] 1-2 nemici o pericoli ambientali
- [ ] Musica e SFX base
- ✅ Checkpoint: 3-5 minuti giocabili, provati da almeno 5 persone
- [ ] **Apri la pagina Steam** (Coming Soon) per le wishlist

## Fase 6 — Demo (2-3 settimane)
- [ ] 2-3 livelli + mini-boss o sequenza di fuga
- [ ] Menu, opzioni, rimappatura comandi, supporto gamepad
- [ ] Trailer (30-60 s), screenshot, capsule art
- [ ] Questionario contenuti Steam compilato onestamente
- [ ] Iscrizione a Steam Next Fest e Anthro Festival
- ✅ Checkpoint: demo pubblicata

**Totale stimato: circa 3-4 mesi part-time.**
