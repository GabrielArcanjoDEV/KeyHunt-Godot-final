# Notas técnicas da migração

## Base analisada
Projeto Java Swing/Java2D "Key Hunt". O ZIP recebido continha:
- `Tela`: janela principal de 1280x720.
- `PainelJogo`, `MenuGame`, `PainelMapa`, `TelaOpcoes`, `TelaDerrota`, `TelaVencedor`.
- `Jogador`, `Inimigo`, `Entidade`, `Camera`.
- `MapaMatriz`, `GeradorMapa`, `Decoracao`.
- Imagens, spritesheets, tiles e WAVs.

## Preservado
- Matriz 22x31 do mapa e matriz 22x31 de decoração.
- Tile size 128.
- Spawn jogador (linha 3, coluna 2).
- Spawns inimigos (7,17), (10,18), (18,18).
- Sete posições de chaves.
- Sprite sheet jogador (4 direções x 8 frames).
- Sequências Golem Idle (18), Running (12), Kicking (12).
- Vida do jogador (5), vida do inimigo (3), velocidade nominal do jogador (380).
- Telas e áudios originais.

## Ajustes pendentes para fidelidade total
1. Gerar colisores estáticos para tiles sólidos e decorações sólidas.
2. Implementar iluminação radial usando Light2D/GradientTexture2D.
3. Revisar alinhamento/escala dos sprites no runtime e testar colisões.
4. Ajustar IA do inimigo e janela exata de dano para equivaler ao Java.
5. Ajustar preferências de áudio para persistirem entre cenas.

A versão entregue fornece uma base organizada e editável para continuar a migração na Godot. A execução final precisa ser testada no Godot 4 .NET instalado no Windows.
