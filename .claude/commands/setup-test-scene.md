---
description: Crea la scena di test del movimento (capsula + livello grey-box)
---

Crea una scena di test per il movimento usando **Unity MCP**. Segui `CLAUDE.md`.

1. **Verifica**: Unity MCP connesso, pacchetti Input System / Cinemachine / ProBuilder installati, Console senza errori di compilazione. Se manca qualcosa, fermati e dimmelo.
2. **Scena**: crea `Assets/_Project/Scenes/TestMovement.unity` e aprila. Directional Light + Global Volume di default URP.
3. **Player**:
   - GameObject `Player` a (0, 1, 0) con `CharacterController` (height 1.8, radius 0.4, center (0, 0.9, 0)) e `Project.Player.PlayerMotor`.
   - Figlio `Visual`: capsula (senza collider) + `Project.Player.PlayerVisuals` con `motor` assegnato. Un piccolo cubo figlio come "naso" per vedere la direzione.
   - Salva come prefab in `Assets/_Project/Prefabs/Player.prefab`.
4. **Livello grey-box** (ProBuilder o cubi, tutto su Z = 0, layer `Ground`):
   - Pavimento lungo ~60 m.
   - Piattaforme a altezze crescenti (gap che richiedono salto, doppio salto e dash).
   - Due muri alti e paralleli a ~3 m per provare il salto a muro.
   - Qualche decorazione a Z = +3 e Z = -4 (senza collider) per la profondità.
   - Imposta `wallMask` di `PlayerMotor` sul layer `Ground`.
5. **Camera**: Cinemachine Camera che segue `Player`, Position Composer, distanza ~14, lieve dead zone e look-ahead. Main Camera con `CinemachineBrain`.
6. **Materiali** semplici in `Assets/_Project/Materials/` (grigio per il livello, colore acceso per il player).
7. **Controllo**: salva la scena, leggi la Console, avvia Play mode per qualche secondo e verifica che non ci siano errori. Riporta l'esito.

Alla fine spunta la voce corrispondente in `docs/ROADMAP.md`.
